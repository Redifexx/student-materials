using Robocode;
using System;

namespace CAP4053.Student
{
    public class RediBot : TeamRobot
    {
        override public void Run()
        {
            Console.WriteLine("RUNNING");
            while (true)
            {
                Ahead(100);
                TurnRight(90);
                Back(100);
                TurnGunRight(360);
            }
        }

        override public void OnScannedRobot(ScannedRobotEvent e)
        {
            Fire(1);
        }

    }
}
