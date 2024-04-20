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
        public class Bot
        {
            public string name;
            public double energy = -1;
            public double distance = -1;
            public double heading = -1;
            public double velocity = -1;
            public double posX = -1;
            public double posY = -1;
            public double velX = -1;
            public double velY = -1;
            public int bulletHits = 0;
            public int bulletsReceived = 0;
            public Bot(string name_)
            {
                name = name_;
            }

            // For Easy Search
            //public override bool Equals(object obj)
            //{
            //    if (!(obj is Bot))
            //    {
            //        return false;
            //    }
            //    Bot other = (Bot)obj;
            //    return name == other.name;
            //}
            //
            ////GPT told me there was an issue with my hash code and this was the fix (sorry this is beyond the scope of this proj ;P)
            //public override int GetHashCode()
            //{
            //    // Combine hash codes of relevant properties
            //    unchecked // Overflow is fine, just wrap
            //    {
            //        int hashCode = 17; // Prime number
            //        hashCode = hashCode * 23 + posX.GetHashCode();
            //        hashCode = hashCode * 23 + posY.GetHashCode();
            //        // Add other relevant properties here
            //        return hashCode;
            //    }
            //}
        }

        // Key Variables-----------------

        Dictionary<string, Bot> enemyNames = new Dictionary<string, Bot>();
        List<Bot> enemyBots = new List<Bot>();
        Bot target = new Bot("null");


        // States------------------------

        public bool isOneOnOne = false;
        public bool isIdle = true;
        public bool isCurious = false;
        public bool isScanning = false;
        public bool isRadarLocked = false;
        public bool isAimLocked = false;
        public bool hasTarget = false;
        public bool isSnaking = false;

        public double shootAheadStrength = 0.5;
        public int bulletsShot = 0;

        public int lastDirection = 1;
        public int zigZagDirection = 1;
        public int DEFCON_ = 5;


        // Actions ----------------------

        public void scanEnv()
        {
            Console.WriteLine("scanEnv()");
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            isScanning = true;
            TurnRadarRight(360);
            isScanning = false;
        }

        // Same as scan but Non Blocking
        public void searchEnv()
        {
            Console.WriteLine("searchEnv()");
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            isScanning = true;
            SetTurnRadarRight(360);
            isScanning = false;
        }

        public void setTarget()
        {
            Console.WriteLine("setTarget()");
            double lowestEnergy = 500.0;
            foreach (Bot bot in enemyBots)
            {
                if (target.name == "null" || bot.energy < lowestEnergy)
                {
                    lowestEnergy = bot.energy;
                    target = bot;
                    Console.WriteLine("Target Name: " + target.name);
                    Console.WriteLine("Target Energy: " + target.energy);
                    Console.WriteLine("Target Distance: " + target.distance);
                    Console.WriteLine("Target Pos X " + target.posX);
                    Console.WriteLine("Target Pos Y " + target.posY);
                }
            }
            if (!(target.name == "null"))
            {
                hasTarget = true;
            }
        }

        public void faceTarget(double offset)
        {
            Console.WriteLine("faceTarget()");
            //WiFi just went down so I had to pull out Soh Cah Toa for this (im proud of myself)
            Console.WriteLine("Target Name: " + target.name);
            Console.WriteLine("Target Energy: " + target.energy);
            Console.WriteLine("Target Distance: " + target.distance);
            Console.WriteLine("Target Pos X " + target.posX);
            Console.WriteLine("Target Pos Y " + target.posX);
            double deltaX = (target.posX - this.X);
            double deltaY = (target.posY - this.Y);
            double angleToFace = Math.Atan2(deltaX, deltaY);
            double desiredAngle = Utils.NormalRelativeAngle(angleToFace - this.HeadingRadians) + offset;
            SetTurnRightRadians(desiredAngle);
        }

        public void aimTarget()
        {
            Console.WriteLine("aimTarget()");
            //WiFi just went down so I had to pull out Soh Cah Toa for this (im proud of myself)
            double deltaX = (target.posX - this.X);
            double deltaY = (target.posY - this.Y);
            double angleToFace = Math.Atan2(deltaX, deltaY);
            double desiredAngle = Utils.NormalRelativeAngle(angleToFace - this.GunHeadingRadians);
            IsAdjustGunForRobotTurn = true;
            SetTurnGunRightRadians(desiredAngle);
        }

        public void approachTarget(double distance_)
        {
            if (!isSnaking)
            {
                faceTarget(0);
            }
            aimTarget();
            if (target.distance > distance_)
            {
                if (isSnaking)
                {
                    faceTarget(45);
                    SetAhead(100);
                    faceTarget(-45);
                    SetAhead(100);
                }
                else
                {
                    SetAhead(100);
                }
            }
            
        }

        public void avoidTarget(double distance_)
        {
            faceTarget(0);
            aimTarget();
            if (target.distance < distance_)
            {
                SetBack(100);
            }
        }

        public void keepDistanceTarget(double distance_)
        {
            if (target.distance > distance_)
            {
                approachTarget(distance_);
            }
            else if (target.distance < distance_)
            {
                avoidTarget(distance_);
            }
        }

        public void zigZagTarget()
        {
            faceTarget(90);
            aimTarget();
            //if (Time % 50 == 0)
            //{
            //    zigZagDirection *= -1;
            //}
            SetAhead(200);
            //SetBack(200);
        }
        public void readTarget()
        {
            Console.WriteLine("readTarget()");
            //WiFi just went down so I had to pull out Soh Cah Toa for this (im proud of myself)
            double deltaX = (target.posX - this.X);
            double deltaY = (target.posY - this.Y);
            double angleToFace = Math.Atan2(deltaX, deltaY);
            double desiredAngle = Utils.NormalRelativeAngle(angleToFace - this.RadarHeadingRadians);
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            SetTurnRadarRightRadians(desiredAngle);
        }

        public void checkBehaviorState()
        {
            if (DEFCON_ == 5)
            {
                isCurious = true;
            }
            else if (DEFCON_ == 4)
            {
                DEFCON_ = 5;
            }
            else if (DEFCON_ == 3)
            {
                DEFCON_ = 4;
            }
            else if (DEFCON_ == 2)
            {
                DEFCON_ = 3;
            }
            else if (DEFCON_ == 1)
            {
                DEFCON_ = 2;
            }
        }

        public void updateDEFCON()
        {
            if (this.Energy > 50)
            {
                DEFCON_ = 5;
            }
            else if (this.Energy > 40)
            {
                DEFCON_ = 4;
            }
            else if(this.Energy > 30)
            {
                DEFCON_ = 3;
            }
            else if (this.Energy > 20)
            {
                DEFCON_ = 2;
            }
            else if (this.Energy > 10)
            {
                DEFCON_ = 1;
            }
            Console.WriteLine("DEFCON: " + DEFCON_);
        }

        public void shoot()
        {
            int firePower = 0;
            if (target.distance > 600)
            {
                firePower = 1;
            }
            else if (target.distance > 400)
            {
                firePower = 2;
            }
            else if (target.distance > 200)
            {
                firePower = 3;
            }
            else
            {
                firePower = 4;
            }

            //Get the distance to determine how long it would take to get there
            double shootAheadFactor = 1;
            if (bulletsShot > 0)
            {
                shootAheadFactor = shootAheadStrength / bulletsShot;
            }
            //Judges Bullet Accuracy

            double bulletTime = (target.distance * shootAheadFactor) / (20 - (3 * firePower));
            Console.WriteLine(shootAheadFactor);

            double futureX = target.posX + target.velocity * bulletTime * Math.Sin(target.heading);
            double futureY = target.posY + target.velocity * bulletTime * Math.Cos(target.heading);
            double myFutureX = this.X + this.Velocity * bulletTime * Math.Sin(this.HeadingRadians);
            double myFutureY = this.Y + this.Velocity * bulletTime * Math.Cos(this.HeadingRadians);

            //double curDirX = futureX - target.posX;
            //double curDirY = futureY - target.posY;
            double curDirX = futureX - myFutureX;
            double curDirY = futureY - myFutureY;

            double angleToOp = Math.Atan2(curDirX, curDirY);
            SetTurnGunRightRadians(Utils.NormalRelativeAngle(angleToOp - this.GunHeadingRadians));

            Fire(firePower);
            bulletsShot++;
        }

        public void updateBotData(string name_, double energy_, double distance_, double? headingRad_, double? bearingRad_, double? velocity_)
        {
            Bot bot; // i love c# garbage collection <3
            if (!enemyNames.TryGetValue(name_, out bot))
            {
                // Bot does not exist in the dictionary, create a new one
                Console.WriteLine("Adding " + name_);
                bot = new Bot(name_);
                enemyNames[name_] = bot;
                enemyBots.Add(bot);
            }

            bot.energy = energy_;
            bot.distance = distance_;

            // Use Heading and Bearing to approximate forward vector and position
            if (headingRad_.HasValue && bearingRad_.HasValue && velocity_.HasValue)
            {
                // Get Position using Cos and Sin
                double angleToEnemy = this.HeadingRadians + bearingRad_.Value;
                //Console.WriteLine("Angle To Enemy: " + (angleToEnemy * (180 / Math.PI)));
                bot.posX = ((distance_ * Math.Sin(angleToEnemy)) + this.X);
                bot.posY = ((distance_ * Math.Cos(angleToEnemy)) + this.Y);

                // Get Forward Vector
                bot.velX = velocity_.Value * Math.Sin(angleToEnemy);
                bot.velY = velocity_.Value * Math.Cos(angleToEnemy);
                bot.heading = headingRad_.Value;
                bot.velocity = velocity_.Value;
            }
            //Console.WriteLine("Added " + bot.name);
            //Console.WriteLine("Energy " + bot.energy);
            //Console.WriteLine("Distance " + bot.distance);
            Console.WriteLine("Pos X " + bot.posX);
            Console.WriteLine("Pos Y " + bot.posY);
            Console.WriteLine("Updated!");
        }


        public void stateSelector()
        {
            updateDEFCON();

            if (isIdle)
            {
                SetTurnRight(10);

                if (hasTarget)
                {
                    Console.WriteLine("Has Target!");
                    isIdle = false;
                    isCurious = true;
                }
            }

            if (!isRadarLocked && !isIdle)
            {
                searchEnv();
                readTarget();
            }

            if (isCurious)
            {
                keepDistanceTarget(500);
                if (target.distance < 400)
                {
                    isCurious = false;
                }
            }
            else
            {
                if (Time % 1000 == 0)
                {
                    checkBehaviorState();
                }
                if (DEFCON_ == 5)
                {
                    keepDistanceTarget(200);
                    //isSnaking = false;
                    //zigZagTarget();
                    shoot();
                }
                else if(DEFCON_ == 4)
                {
                    keepDistanceTarget(400);
                    //isSnaking = true;
                    //zigZagTarget();
                    shoot();
                }
                else if (DEFCON_ == 3)
                {
                    keepDistanceTarget(600);
                    zigZagTarget();
                    shoot();
                }
                else if (DEFCON_ == 2)
                {
                    zigZagTarget();
                    shoot();
                }
                else if (DEFCON_ == 1)
                {
                    zigZagTarget();
                    shoot();
                }
            }
        }

      
        override public void Run()
        {
            scanEnv();
            setTarget();
            if (Others == 1)
            {
                Console.WriteLine("One on One!");
                isOneOnOne = true;
            }
            while (true)
            {
                if (!isOneOnOne)
                {
                    if (Time % 3 == 0)
                    {
                        scanEnv();
                        setTarget();
                    }
                }
                Execute();
                stateSelector();
            }
        }

        override public void OnScannedRobot(ScannedRobotEvent e)
        {
            updateBotData(e.Name, e.Energy, e.Distance, e.HeadingRadians, e.BearingRadians, e.Velocity);
            if (isOneOnOne)
            {
                SetTurnRadarRight(2.0 * Utils.NormalRelativeAngleDegrees(this.Heading + e.Bearing - this.RadarHeading));
            }
            if (Math.Abs(2.0 * Utils.NormalRelativeAngleDegrees(this.Heading + e.Bearing - this.RadarHeading)) > 5)
            {
                isRadarLocked = false;
            }
            else
            {
                isRadarLocked = true;
            }
        }

        override public void OnBulletHit(BulletHitEvent e)
        {
            shootAheadStrength += 1;

        }

        override public void OnBulletHitBullet(BulletHitBulletEvent e)
        {
            
        }

        override public void OnBulletMissed(BulletMissedEvent e)
        {

        }

        override public void OnHitByBullet(HitByBulletEvent e)
        {
            //SetTurnRight(e.Bearing);
            isCurious = false;
            zigZagDirection *= -1;
        }

        override public void OnHitRobot(HitRobotEvent e)
        {
            //SetTurnRight(e.Bearing);
        }

        override public void OnHitWall(HitWallEvent e)
        {
            //if (e.Bearing < 0)
            //{
            //    TurnLeft(Utils.NormalAbsoluteAngleDegrees(e.Bearing - 90));
            //}
            //else
            //{
            //    TurnRight(Utils.NormalAbsoluteAngleDegrees(e.Bearing + 90));
            //}
            //Ahead(300);
            //zigZagDirection *= -1;
            //Back(100);
            //TurnRight(90);
            //TurnGunRight(360);

        }
    }
}
