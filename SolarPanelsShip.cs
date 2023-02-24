using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Text.RegularExpressions;
namespace SolarPanelsShip
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        IMyCameraBlock Cam;
        IMyGyro Gyro;
        IMySolarPanel Panel;
        Vector3D V1, V2, Axis;

        public Program()
        {
            Cam = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            Gyro = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;
            Panel = GridTerminalSystem.GetBlockWithName("Solar Panel") as IMySolarPanel;
        }

        public void Main(string argument)
        {
            switch (argument)
            {
                case "V1":
                    {
                        V1 = Cam.WorldMatrix.Forward;
                        break;
                    }
                case "V2":
                    {
                        V2 = Cam.WorldMatrix.Forward;
                        Axis = V1.Cross(V2);
                        Axis = Vector3D.Normalize(Axis);
                        break;
                    }
                case "Start":
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update1;
                        Gyro.GyroOverride = true;
                        TrackSun();
                        break;
                    }
                case "Stop":
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        Gyro.GyroOverride = false;
                        break;
                    }

                default:
                    TrackSun();
                    break;
            }
        }

        float SolarOutput = 0;
        int dir = 1;
        int cnt = 0;
        void TrackSun()
        {
            cnt++;
            Gyro.Pitch = -(float)Axis.Dot(Gyro.WorldMatrix.Up) * 5;
            Gyro.Yaw = -(float)Axis.Dot(Gyro.WorldMatrix.Left) * 5;
            if (Math.Abs(Gyro.Pitch) + Math.Abs(Gyro.Yaw) < 0.05f)
            {
                if (cnt >= 180)
                {
                    float OutputGain = Panel.MaxOutput - SolarOutput;
                    if (OutputGain < 0)
                        dir *= -1;
                    SolarOutput = Panel.MaxOutput;
                    cnt = 0;
                }
                float Roll = (float)(dir * (0.03f - Panel.MaxOutput) * 100);
                Gyro.Roll = Math.Min(Math.Max(Roll, -0.1f), 0.1f);
            }
            else
            {
                Gyro.Roll = 0.0f;
            }
        }



        public void Save()
        { }

        //------------END--------------
    }
}