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
        GyroControl g1;
        public MyThrusters mt1;
        //string cockpitName = "Drill FlightSeat";
        //public IMyCockpit cockpit;

        public Program()
        {
            myScript = this;

            g1 = new GyroControl();
            mt1 = new MyThrusters();
            //cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

        }   

        public void Main(string argument)
        {
            if (argument == "Start")
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update1;
                g1.GyroOver(true);
            }
            if (argument == "Stop")
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
                mt1.SetGroupThrust(mt1.ForwardThrusters, 0);
                mt1.SetGroupThrust(mt1.BackwardThrusters, 0);
                g1.GyroOver(false);
            }
            else
            {
                mt1.SpeedControl(0.67f);
                g1.KeepHorizon();
            }
        }

        public class GyroControl
        {
            List<IMyGyro> gyrolist;
            //string cockpitName = "Drill FlightSeat";
            //IMyCockpit cockpit;
            string contrName = "Drill FlightSeat";
            public IMyCockpit controller;

            public GyroControl()
            {
                gyrolist = new List<IMyGyro>();
                //cockpit = myScript.GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
                controller = myScript.GridTerminalSystem.GetBlockWithName(contrName) as IMyCockpit;
                //myScript.GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyrolist, (a) => (a.IsSameConstructAs(cockpit)));
                //myScript.GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyrolist, (a) => (a.IsSameConstructAs(controller)));
                myScript.GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyrolist);
            }


            public void KeepHorizon()
            {
                //Vector3D grav = Vector3D.Normalize(cockpit.GetNaturalGravity());
                //Vector3D axis = grav.Cross(cockpit.WorldMatrix.Down);

                Vector3D grav = Vector3D.Normalize(controller.GetNaturalGravity());
                Vector3D axis = grav.Cross(controller.WorldMatrix.Down);


                //if (grav.Dot(cockpit.WorldMatrix.Down) < 0)
                //{
                //    axis = Vector3D.Normalize(axis);
                //}

                if (grav.Dot(controller.WorldMatrix.Down) < 0)
                {
                    axis = Vector3D.Normalize(axis);
                }
                SetGyro(axis);
            }

            public void SetGyro(Vector3D axis)
            {
                foreach (IMyGyro gyro in gyrolist)
                {
                    gyro.Yaw = (float)axis.Dot(gyro.WorldMatrix.Up);
                    gyro.Pitch = (float)axis.Dot(gyro.WorldMatrix.Right);
                    gyro.Roll = (float)axis.Dot(gyro.WorldMatrix.Backward);
                }
            }

            public void GyroOver(bool over)
            {
                foreach (IMyGyro gyro in gyrolist)
                {
                    gyro.Yaw = 0;
                    gyro.Pitch = 0;
                    gyro.Roll = 0;
                    gyro.GyroOverride = over;
                }
            }




        }

        //public class BlocksStatus
        //{
        //    float status;
        //    List<IMyTerminalBlock> blocks;
        //    IMyCockpit contrl;
        //    string contrlName = "Drill FlightSeat";
        //    public BlocksStatus()
        //    {
        //        blocks = new List<IMyTerminalBlock>();
        //        myScript.GridTerminalSystem.GetBlocks(blocks);
        //        contrl = myScript.GridTerminalSystem.GetBlockWithName(contrlName) as IMyCockpit;
        //        contrl.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
        //        contrl.GetSurface(0).WriteText("", false);
        //    }

        //    public void GetStatus()
        //    {
        //        foreach (IMyTerminalBlock block in blocks)
        //        {
        //            status = block.DisassembleRatio;
        //            //contrl.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
        //            contrl.GetSurface(0).WriteText(block.Name + ": " + status.ToString() + "\n", true);
        //        }
        //    }


        //}

        public class MyThrusters
        {

            string cockpitName = "Drill FlightSeat";
            IMyCockpit cockpit;

            List<IMyThrust> AllThrusters;
            List<IMyThrust> UpThrusters;
            List<IMyThrust> DownThrusters;
            List<IMyThrust> LeftThrusters;
            List<IMyThrust> RightThrusters;
            public List<IMyThrust> ForwardThrusters;
            public List<IMyThrust> BackwardThrusters;

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

            public void SpeedControl (float Spd)
            {
                double shipSpeed;
                shipSpeed = cockpit.GetShipSpeed();
                if (shipSpeed < Spd)
                {
                    SetGroupThrust(this.ForwardThrusters, 0.01f);
                    SetGroupThrust(this.BackwardThrusters, 0.0f);
                }
                else if (shipSpeed >= Spd)
                {
                    SetGroupThrust(this.ForwardThrusters, 0.0f);
                    SetGroupThrust(this.BackwardThrusters, 0.01f);
                }
            }

            public void SetGroupThrust(List<IMyThrust> ThrList, float Thr)
            {

                for (int i = 0; i < ThrList.Count; i++)
                {
                    //ThrList[i].SetValue("Override", Thr); //OldSchool
                    ThrList[i].ThrustOverridePercentage = Thr;
                }
                //cockpit.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
                //cockpit.GetSurface(0).WriteText(tickCount.ToString(), false);
            }
        }

        //------------END--------------
    }

}
