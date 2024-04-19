using Robocode;
using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace CAP4053.Student
{
    public class RediBot : TeamRobot
    {
        public struct Bot
        {
            public string botName;
            public double lastKnownEnergy;
            public double lastKnownDistance;
            public double lastKnownVelocity;
            public double lastKnownX;
            public double lastKnownY;
            public int bulletsSent;
            public int bulletsReceived;
            public Bot(string botName_)
            {
                botName = botName_;
            }
        }

        List<Bot> enemyBots = new List<Bot>();
        Dictionary<string, Bot> enemyNames = new Dictionary<string, Bot>();
        //List<string> enemyNames = new List<string>(); //Parallel list so I don't have to iterate over bots to find it
        Bot curTarget = new Bot("null");

        //States
        public bool isScanning = false;
        public bool hasTarget = false;
        public bool isFacingTarget = false;
        public bool isInRange = false;

        public void scanEnv()
        {
            Console.WriteLine("Scanning environment.");
            isScanning = true;
            TurnRight(360);
            if (!isFacingTarget)
            {
                Console.WriteLine("SCAN");
                Scan();
            }
            isScanning = false;
            Console.WriteLine("Finished!");
        }

        public void setTarget()
        {
            Console.WriteLine("Calculating Target.");
            //Closest Target
            double closestBot = -1.0;
            foreach (Bot bot in enemyBots)
            {
                Console.WriteLine("LOOP");
                if (curTarget.botName == "null" || bot.lastKnownDistance < closestBot)
                {
                    closestBot = bot.lastKnownDistance;
                    curTarget = bot;
                }
            }
            Console.WriteLine("Current Target: " + curTarget.botName);
        }

        public void circleTarget()
        {
            if (curTarget.botName != "null")
            {
                double radius = curTarget.lastKnownDistance;
                double angleToTarget = Math.Atan2(curTarget.lastKnownY - this.Y, curTarget.lastKnownX - this.X);
                double centerX = curTarget.lastKnownX - Math.Sin(angleToTarget) * radius;
                double centerY = curTarget.lastKnownY - Math.Cos(angleToTarget) * radius;

                // Calculate the angle to the circle center
                double angleToCenter = Math.Atan2(centerY - Y, centerX - X);
                // Calculate the distance to the circle center
                double distanceToCenter = Math.Sqrt((centerX - X) * (centerX - X) + (centerY - Y) * (centerY - Y));
                SetTurnRightRadians(Utils.NormalRelativeAngle(angleToCenter - HeadingRadians));
                SetAhead(distanceToCenter);

            }
        }

        public void shootTarget(double distance_)
        {
            Fire(1);
        }

        override public void Run()
        {
            scanEnv();
            while (true)
            {
                if (isScanning)
                {
                    scanEnv();
                }
                if (isFacingTarget)
                {
                    if (!isInRange)
                    {
                        Ahead(100);
                    }
                }
            }
        }

        override public void OnScannedRobot(ScannedRobotEvent e)
        {
            //isScanning = false;
            Console.WriteLine("Enemy Bearing: " + e.Bearing);
            TurnRight(e.Bearing);
            if (Math.Abs(e.Bearing) < 0.01)
            {
                isFacingTarget = true;
            }
            if (isFacingTarget)
            {
                Console.WriteLine("Moving Forward!");
                //Ahead(e.Distance / 2);
                if (e.Distance < 100)
                {
                    isInRange = true;
                }
                else
                {
                    isInRange = false;
                }
                Bot curBot;
                if (!enemyNames.TryGetValue(e.Name, out curBot))
                {
                    Console.WriteLine("Adding " + e.Name);
                    curBot = new Bot(e.Name);
                    enemyNames[e.Name] = curBot;
                    curBot.bulletsReceived = 0;
                    curBot.bulletsSent = 0;
                    enemyBots.Add(curBot);
                }
                //Update Bot Values
                curBot.lastKnownEnergy = e.Energy;
                curBot.lastKnownDistance = e.Distance;
                curBot.lastKnownVelocity = e.Velocity;
                double enemyBearing = (this.HeadingRadians + e.BearingRadians) % (2 * Math.PI);


                curBot.lastKnownX = this.X + (e.Distance * Math.Cos(enemyBearing));
                curBot.lastKnownY = this.Y + (e.Distance * Math.Sin(enemyBearing));
                Console.WriteLine("Dist: " + e.Distance);
                Console.WriteLine("Enemy Bearing: " + enemyBearing);
                Console.WriteLine("Target X: " + curBot.lastKnownX);
                Console.WriteLine("Target Y: " + curBot.lastKnownY);
                if (!isScanning)
                {
                    setTarget();
                }
            } 
        }

        //When it sees another robot in the direction of the gun
        /*
        override public void OnScannedRobot(ScannedRobotEvent e)
        {
            isScanning = false;
            Console.WriteLine("E BEARING RAD:" + e.BearingRadians);
            if (true)
            {
                while (Math.Abs(e.Bearing) > 0.02)
                {
                    TurnRight(Math.Abs(e.Bearing));
                    Execute(); // Execute the turn immediately
                }

                Console.WriteLine("Scanned Bot.");
                Bot curBot;
                if (!enemyNames.TryGetValue(e.Name, out curBot))
                {
                    Console.WriteLine("Adding " + e.Name);
                    curBot = new Bot(e.Name);
                    enemyNames[e.Name] = curBot;
                    curBot.bulletsReceived = 0;
                    curBot.bulletsSent = 0;
                    enemyBots.Add(curBot);
                }
                //Update Bot Values
                curBot.lastKnownEnergy = e.Energy;
                curBot.lastKnownDistance = e.Distance;
                curBot.lastKnownVelocity = e.Velocity;
                double enemyBearing = (this.HeadingRadians + e.BearingRadians) % (2 * Math.PI);


                curBot.lastKnownX = this.X + (e.Distance * Math.Cos(enemyBearing));
                curBot.lastKnownY = this.Y + (e.Distance * Math.Sin(enemyBearing));
                Console.WriteLine("Dist: " + e.Distance);
                Console.WriteLine("Enemy Bearing: " + enemyBearing);
                Console.WriteLine("Target X: " + curBot.lastKnownX);
                Console.WriteLine("Target Y: " + curBot.lastKnownY);
                if (!isScanning)
                {
                    setTarget();
                    faceTarget();
                }
            }
        }
        */

        override public void OnBulletHit(BulletHitEvent e)
        {
            
        }

        override public void OnBulletHitBullet(BulletHitBulletEvent e)
        {
            
        }

        override public void OnBulletMissed(BulletMissedEvent e)
        {

        }

        override public void OnHitByBullet(HitByBulletEvent e)
        {

        }

        override public void OnHitRobot(HitRobotEvent e)
        {

        }

        override public void OnHitWall(HitWallEvent e)
        {
            //Back(100);
            //TurnRight(90);
            //TurnGunRight(360);

        }

        public new void Scan()
        {

        }
    }
}
