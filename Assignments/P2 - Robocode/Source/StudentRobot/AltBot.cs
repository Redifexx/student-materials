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
    public class AltBot : TeamRobot
    {
        public struct Bot
        {
            public string botName;
            public double roughEnergy;
            public double roughDistance;
            public double roughHeading;
            public double roughVelocity;

            public double roughDirX;
            public double roughDirY;
            public double roughX;
            public double roughY;
            //public int bulletsSent;
            //public int bulletsReceived;
            public Bot(string botName_)
            {
                botName = botName_;
            }
        }

        List<Bot> enemyBots = new List<Bot>();
        Dictionary<string, Bot> enemyNames = new Dictionary<string, Bot>();
        Bot curTarget = new Bot("null");

        //States
        public bool isScanning = false;
        public bool hasTarget = false;
        public bool isFacingTarget = false;
        public bool isInRange = false;

        //Rules i guess
        public double farDistance = 600;
        public string lastBotScanned = "null";
        public int lastBotScannedAmt = 1;
        public double lastKnownBearing = 0;
        public int turnDirection = 1;

        public void scanEnv()
        {
            Console.WriteLine("scanEnv()");
            isScanning = true;
            TurnRight(360);
            isScanning = false;
            Console.WriteLine("Finished!");
        }

        public void setTarget()
        {
            Console.WriteLine("setTarget()");
            //Closest Target
            double closestBot = -1.0;
            foreach (Bot bot in enemyBots)
            {
                if (curTarget.botName == "null" || bot.roughDistance < closestBot)
                {
                    Console.WriteLine("New Bot is: " + bot.botName);
                    closestBot = bot.roughDistance;
                    curTarget = bot;
                    hasTarget = true;
                }
            }
            Console.WriteLine("Current Target: " + curTarget.botName + " with heading " + curTarget.roughHeading);
        }

        public void lookAtTarget()
        {
            Console.WriteLine("lookAtTarget()");
            double targetHeading = curTarget.roughHeading / lastBotScannedAmt;
            Console.WriteLine(curTarget.botName + " " + curTarget.roughHeading + " " + curTarget.roughEnergy + " " + curTarget.roughDistance);
            TurnRightRadians(Utils.NormalRelativeAngle(targetHeading - this.HeadingRadians));
            //Scan();
        }

        public void shoot(ScannedRobotEvent e)
        {
            //Get the distance to determine how long it would take to get there
            double bulletTime = e.Distance / 20.0;

            double futureX = curTarget.roughX + curTarget.roughVelocity * bulletTime * Math.Sin(curTarget.roughHeading);
            double futureY = curTarget.roughY + curTarget.roughVelocity * bulletTime * Math.Cos(curTarget.roughHeading);

            double curDirX = futureX - curTarget.roughX;
            double curDirY = futureY - curTarget.roughY;

            double angleToOp = Math.Atan2(curDirX, curDirY);
            SetTurnGunRightRadians(Utils.NormalRelativeAngle(angleToOp - this.GunHeadingRadians));

            if (e.Distance > 600)
            {
                Fire(1);
            }
            else if (e.Distance > 400)
            {
                Fire(2);
            }
            else if (e.Distance > 200)
            {
                Fire(3);
            }
            else
            {
                Fire(4);
            }
        }

        public void updateBots(ScannedRobotEvent e)
        {
            Bot curBot;
            if (!enemyNames.TryGetValue(e.Name, out curBot))
            {
                // Bot does not exist in the dictionary, create a new one
                Console.WriteLine("Adding " + e.Name);
                curBot = new Bot(e.Name);
                enemyNames[e.Name] = curBot;
            }

            // Update Bot Values
            if (lastBotScanned == e.Name)
            {
                curBot.roughHeading += this.HeadingRadians;
                lastBotScannedAmt++;
            }
            else
            {
                curBot.roughHeading = this.HeadingRadians;
                lastBotScannedAmt = 1;
            }
            curBot.roughEnergy = e.Energy;
            curBot.roughDistance = e.Distance;
            curBot.roughVelocity = e.Velocity;

            //Get Approx X Y Pos
            double absoluteBearing = this.HeadingRadians + e.BearingRadians;
            curBot.roughX = this.X + e.Distance * Math.Sin(absoluteBearing);
            curBot.roughY = this.Y + e.Distance * Math.Cos(absoluteBearing);

            //Get Approx X Y Direction Vector
            double futureX = curTarget.roughX + curBot.roughVelocity * 1.0 * Math.Sin(curBot.roughHeading);
            double futureY = curTarget.roughY + curBot.roughVelocity * 1.0 * Math.Cos(curBot.roughHeading);

            curBot.roughDirX = futureX - curBot.roughX;
            curBot.roughDirY = futureY - curBot.roughY;

            Console.WriteLine("Direction X Y: " + curBot.roughDirX + "  " + curBot.roughDirY);

            lastBotScanned = e.Name;


            // Add/update the bot in the enemyBots list if necessary
            if (!enemyBots.Contains(curBot))
            {
                enemyBots.Add(curBot);
            }
        }

        public void stateSelector()
        {
            if (!hasTarget)
            {
                scanEnv(); //Can be inturupted
            }
            else
            {
                SetAhead(100);
                //TurnRight(lastKnownBearing);
                //if (!isFacingTarget)
                //{
                //    lookAtTarget();
                //}
                //Ahead(10);
                //Execute();
            }
        }

        override public void Run()
        {
            while (true)
            {
                stateSelector();
                Execute();
            }
        }

        override public void OnScannedRobot(ScannedRobotEvent e)
        {
            //Console.WriteLine("OnScannedRobot()");
            SetTurnRight(e.Bearing);
            //Fire(4);
            lastKnownBearing = e.Bearing;
            if (Math.Abs(e.Bearing) > 5)
            {
                isFacingTarget = false;
            }

            //Code from Robocode Crash Course 08 from Professor Spensor on YT to tell if it's left or right
            if (e.Bearing >= 0)
            {
                turnDirection = 1;
            }
            else
            {
                turnDirection = -1;
            }

            if (isScanning)
            {
                setTarget();
            }
            updateBots(e);
            shoot(e);
            if (!isFacingTarget)
            {
                if (Math.Abs(e.Bearing) < 0.1)
                {
                    isFacingTarget = true;
                }
            }
        }

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
            SetTurnRight(e.Bearing);
        }

        override public void OnHitRobot(HitRobotEvent e)
        {
            SetTurnRight(e.Bearing);
        }

        override public void OnHitWall(HitWallEvent e)
        {
            //Back(100);
            //TurnRight(90);
            //TurnGunRight(360);

        }
    }
}
