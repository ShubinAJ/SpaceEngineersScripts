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

namespace MyThrusters
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        static Program myScript;

        public Program()
        {
            myScript = this;
        }

        public void Main(string args)
        {

        }

        class MyThrusters
        {
            string cockpitName = "FlightSeat";
            IMyCockpit cockpit;

            List<IMyThrust> AllThrusters;
            List<IMyThrust> UpThrusters;
            List<IMyThrust> DownThrusters;
            List<IMyThrust> LeftThrusters;
            List<IMyThrust> RightThrusters;
            List<IMyThrust> ForwardThrusters;
            List<IMyThrust> BackwardThrusters;

            double UpThrMax;
            double DownThrMax;
            double LeftThrMax;
            double RightThrMax;
            double ForwardThrMax;
            double BackwardThrMax;

            public MyThrusters()
            {
                InitMainBlocks();
            }
            
            private void InitMainBlocks()
            {



                AllThrusters = new List<IMyThrust>();
                UpThrusters = new List<IMyThrust>();
                DownThrusters = new List<IMyThrust>();
                LeftThrusters = new List<IMyThrust>();
                RightThrusters = new List<IMyThrust>();
                ForwardThrusters = new List<IMyThrust>();
                BackwardThrusters = new List<IMyThrust>();
                UpThrMax = 0;
                DownThrMax = 0;
                LeftThrMax = 0;
                RightThrMax = 0;
                ForwardThrMax = 0;
                BackwardThrMax = 0;

                myScript.GridTerminalSystem.GetBlocksOfType<IMyThrust>(AllThrusters);
                cockpit = myScript.GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;

                Matrix ThrLocM = new Matrix();
                Matrix MainLocM = new Matrix();
                cockpit.Orientation.GetMatrix(out MainLocM);

                for (int i = 0; i < AllThrusters.Count; i++)
                {
                    IMyThrust Thrust = AllThrusters[i];
                    Thrust.Orientation.GetMatrix(out ThrLocM);
                    //Y
                    if (ThrLocM.Backward == MainLocM.Up)
                    {
                        UpThrusters.Add(Thrust);
                        UpThrMax += Thrust.MaxEffectiveThrust;
                    }
                    else if (ThrLocM.Backward == MainLocM.Down)
                    {
                        DownThrusters.Add(Thrust);
                        DownThrMax += Thrust.MaxEffectiveThrust;
                    }
                    //X
                    else if (ThrLocM.Backward == MainLocM.Left)
                    {
                        LeftThrusters.Add(Thrust);
                        LeftThrMax += Thrust.MaxEffectiveThrust;
                    }
                    else if (ThrLocM.Backward == MainLocM.Right)
                    {
                        RightThrusters.Add(Thrust);
                        RightThrMax += Thrust.MaxEffectiveThrust;
                    }
                    //Z
                    else if (ThrLocM.Backward == MainLocM.Forward)
                    {
                        ForwardThrusters.Add(Thrust);
                        ForwardThrMax += Thrust.MaxEffectiveThrust;
                    }
                    else if (ThrLocM.Backward == MainLocM.Backward)
                    {
                        BackwardThrusters.Add(Thrust);
                        BackwardThrMax += Thrust.MaxEffectiveThrust;
                    }
                }

            }

            private void SetGroupThrust(List<IMyThrust> ThrList, float Thr)
            {
                for (int i = 0; i < ThrList.Count; i++)
                {
                    //ThrList[i].SetValue("Override", Thr); //OldSchool
                    ThrList[i].ThrustOverridePercentage = Thr;
                }
            }


        }


        public void Save()
        { }

        //------------END--------------
    }

}
