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
namespace AutoAvoid
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        
        IMyTextSurface screen;
        IMyCockpit cockpit;
        static float K = 0.0001f;
        static Program myScript;
        string cockpitName = "FlightSeat";
        string CameraName = "ForwardCamera";
        public string AllthrustersGroupName = "AllThrusters";
        //string ThrustersUpName = "ThrustersUp";
        //string ThrustersDownName = "ThrustersDown";
        //string ThrustersLeftName = "ThrustersLeft";
        //string ThrustersRightName = "ThrustersRight";
        //string ThrustersForwardName = "ThrustersForward";
        //string ThrustersBackwardName = "ThrustersBackward";
        string ControllerName = "FlightSeat";

        //double SCAN_DISTANCE = 10000;
        //float PITCH = 0;
        //float YAW = 0;
        //private bool firstrun = true;
        //private MyDetectedEntityInfo info;
        //private StringBuilder sb = new StringBuilder();
        //List<IMyThrust> thrusters;
        //IMyShipController control;
        public Program()
        {
            myScript = this;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            //avoidCamera = GridTerminalSystem.GetBlockWithName("AvoidCamera") as IMyCameraBlock;
            cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
            screen = cockpit.GetSurface(0);
            //thrusters = new List<IMyThrust>();
            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters);
           

            //sc = new Screen(this);
        }
        public void Main(string argument)
        {
            //Avoid();
            //IMyTextPanel textPanel = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
            //switch (argument)
            //{
            //case "Start":
            //    {
            //        Runtime.UpdateFrequency = UpdateFrequency.Update10;
            //        //Screen sc = new Screen(new Program());
            //        //sc.Show();
            //        break;
            //    }

            //case "Stop":
            //    {
            //        Runtime.UpdateFrequency = UpdateFrequency.None;
            //        break;
            //    }

            //default:
            //    {
            //        Runtime.UpdateFrequency = UpdateFrequency.Update1;
            //        break;
            //    }

            //}
            
        }
        public class Camera
        {
            IMyCameraBlock avoidCamera;
            double SCAN_DISTANCE = 10000;
            float PITCH = 0;
            float YAW = 0;
            private bool firstrun = true;
            private MyDetectedEntityInfo info;
            private StringBuilder sb = new StringBuilder();
            double distance;

            public Camera (string CamName)
            {
                avoidCamera = myScript.GridTerminalSystem.GetBlockWithName(CamName) as IMyCameraBlock;
            }
            public void Scan()
            {
                Vector3D myVelocity = myScript.cockpit.GetShipVelocities().LinearVelocity;
                Vector3D PilotInput = myScript.cockpit.MoveIndicator;
                

                if (firstrun)
                {
                    firstrun = false;
                    avoidCamera.EnableRaycast = true;
                }
                if (avoidCamera.CanScan(SCAN_DISTANCE))
                {
                    info = avoidCamera.Raycast(SCAN_DISTANCE, PITCH, YAW);
                    distance = Vector3D.Distance(avoidCamera.GetPosition(), info.HitPosition.Value);
                }
                  
            }



        }
        public class MyThrusters
        {



            List<IMyThrust> AllThrusters;
            List<IMyThrust> UpThrusters;
            List<IMyThrust> DownThrusters;
            List<IMyThrust> LeftThrusters;
            List<IMyThrust> RightThrusters;
            List<IMyThrust> ForwardThrusters;
            List<IMyThrust> BackwardThrusters;

            double UpThrMax = 0;
            double DownThrMax = 0;
            double LeftThrMax = 0;
            double RightThrMax = 0;
            double ForwardThrMax = 0;
            double BackwardThrMax = 0;


            public MyThrusters(/*Follower mbt*/)
            {
                //myBot = mbt;
                InitMainBlocks();
            }

            public void AutoThrust(MyDetectedEntityInfo inf, double dist)
            {
                if (!inf.IsEmpty())
                {
                    foreach (IMyThrust myThrust in BackwardThrusters)
                    {
                        myThrust.ThrustOverride = (dist * K * myThrust.MaxThrust);
                    }
                    foreach (IMyThrust myThrust in ForwardThrusters)
                    {
                        myThrust.ThrustOverride = (dist * K * myThrust.MaxThrust);
                    }
                }
            }

            private void InitMainBlocks()
            {

                Matrix ThrLocM = new Matrix();
                Matrix MainLocM = new Matrix();
                myScript.cockpit.Orientation.GetMatrix(out MainLocM);

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

                foreach (IMyThrust thr in AllThrusters)
                {
                    thr.Orientation.GetMatrix(out ThrLocM);

                    if (ThrLocM.Backward == MainLocM.Up)
                    {
                        UpThrusters.Add(thr);
                    }
                    else if (ThrLocM.Backward == MainLocM.Down)
                    {
                        DownThrusters.Add(thr);
                    }
                    else if (ThrLocM.Backward == MainLocM.Left)
                    {
                        LeftThrusters.Add(thr);
                    }
                    else if (ThrLocM.Backward == MainLocM.Right)
                    {
                        RightThrusters.Add(thr);
                    }
                    else if (ThrLocM.Backward == MainLocM.Forward)
                    {
                        ForwardThrusters.Add(thr);
                    }
                    else if (ThrLocM.Backward == MainLocM.Backward)
                    {
                        BackwardThrusters.Add(thr);
                    }

                }
            }


            public void InitStrafe()
            {

            }

            //public AutoAvoidance (string allThr, string upThr, string downThr, string leftThr, string rightThr, string forThr, string backThr)
            //{
            //    List<IMyThrust> AllThrusters = new List<IMyThrust>();
            //    List<IMyThrust> ThrustersUp = new List<IMyThrust>();
            //    List<IMyThrust> ThrustersDown = new List<IMyThrust>();
            //    List<IMyThrust> ThrustersLeft = new List<IMyThrust>();
            //    List<IMyThrust> ThrustersRight = new List<IMyThrust>();
            //    List<IMyThrust> ThrustersForward = new List<IMyThrust>();
            //    List<IMyThrust> ThrustersBackward = new List<IMyThrust>();

            //    //AllThrusters = myScript.GridTerminalSystem.GetBlockGroupWithName(allThr);
            //    //ThrustersUp = new List<IMyThrust>();
            //    //ThrustersDown = new List<IMyThrust>();
            //    //ThrustersLeft = new List<IMyThrust>();
            //    //ThrustersRight = new List<IMyThrust>();
            //    //ThrustersForward = new List<IMyThrust>();
            //    //ThrustersBackward = new List<IMyThrust>();

            //}



        }

        //public void Avoid()
        //{
        //    Vector3D myVelocity = cockpit.GetShipVelocities().LinearVelocity;
        //    Vector3D PilotInput = cockpit.MoveIndicator;


        //    if (firstrun)
        //    {
        //        firstrun = false;
        //        avoidCamera.EnableRaycast = true;
        //    }
        //    if (avoidCamera.CanScan(SCAN_DISTANCE))
        //        info = avoidCamera.Raycast(SCAN_DISTANCE, PITCH, YAW);
        //    foreach (IMyThrust thruster in thrusters) 
        //    {
                
        //    }

        //    sb.Clear();
        //    //sb.Append("EntityID: " + info.EntityId);
        //    //sb.AppendLine();
        //    sb.Append("Name: " + info.Name);
        //    sb.AppendLine();
        //    sb.Append("Type: " + info.Type);
        //    sb.AppendLine();
        //    sb.Append("Velocity: " + info.Velocity.ToString("0.000"));
        //    sb.AppendLine();
        //    sb.Append("Relationship: " + info.Relationship);
        //    sb.AppendLine();
        //    sb.Append("Size: " + info.BoundingBox.Size.ToString("0.000"));
        //    //sb.AppendLine();
        //    //sb.Append("Position: " + info.Position.ToString("0.000"));

        //    if (info.HitPosition.HasValue)
        //    {
        //        sb.AppendLine();
        //        sb.Append("Hit: " + info.HitPosition.Value.ToString("0.000"));
        //        sb.AppendLine();
        //        sb.Append("Distance: " + Vector3D.Distance(avoidCamera.GetPosition(), info.HitPosition.Value).ToString("0.00"));
        //    }

        //    sb.AppendLine();
        //    sb.Append("Range: " + avoidCamera.AvailableScanRange.ToString());
        //    screen.WriteText(sb.ToString());
        //}

        public void Save()
        { }
        //------------END--------------
    }
}