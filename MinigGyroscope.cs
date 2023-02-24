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
using VRage.Game.GUI.TextPanel;

namespace MinigGyroscope
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------
        static Program myScript;
        MinigGyroscope g1;
        static uint tickCount = 0;
        public Program()
        {
            myScript = this;
            g1 = new MinigGyroscope();

        }   

        public void Main(string args)
        {
            switch (args)
            {
                case "Start":
                    {

                        Runtime.UpdateFrequency = UpdateFrequency.Update1;
                        tickCount++;
                        g1.Mining();
                        break;
                    }
                case "Stop":
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        g1.StopMining();
                        break;
                    }
            }
        }
        public class MinigGyroscope
        {
            //List<IMyGyro> Gyroses;
            List<IMyThrust> forwardThrusters;
            string forwThrustGroupName = "BMForwardThrusters";
            IMyBlockGroup forwardThrustersGroup;
            //uint tickCount = 0;

            public MinigGyroscope()
            {

                //Gyroses = new List<IMyGyro>();
                //myScript.GridTerminalSystem.GetBlocksOfType<IMyGyro>(Gyroses);

                forwardThrusters = new List<IMyThrust>();
                forwardThrustersGroup = myScript.GridTerminalSystem.GetBlockGroupWithName(forwThrustGroupName);
                forwardThrustersGroup.GetBlocksOfType<IMyThrust>(forwardThrusters);

            }
        public void Mining()
            {
                ////tickCount++;
                //foreach (IMyGyro gyro in Gyroses)
                //{
                //    gyro.GyroOverride = true;
                //    gyro.Roll = 0.2f;
                //}
                if (tickCount%(60*10)==0)
                {
                   foreach (IMyThrust thruster in forwardThrusters)
                    {
                        thruster.ThrustOverride = 150.0f;
                    }
                }
                else
                {
                    foreach (IMyThrust thruster in forwardThrusters)
                    {
                        thruster.ThrustOverride = 0.0f;
                    }
                }
                if (tickCount >= 4000000000)
                {
                    tickCount = 0;
                }
            }
        public void StopMining()
            {
                //foreach (IMyGyro gyro in Gyroses)
                //{
                //    gyro.GyroOverride = false;
                //    gyro.Roll = 0.0f;
                //}
                foreach (IMyThrust thruster in forwardThrusters)
                {
                    thruster.ThrustOverride = 0.0f;
                }
                tickCount = 0;
            }
        }

        //------------END--------------
    }

}
