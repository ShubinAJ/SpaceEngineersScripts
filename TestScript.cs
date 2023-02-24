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
namespace TestScript
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        double SCAN_DISTANCE = 10000;
        float PITCH = 0;
        float YAW = 0;
        private IMyCameraBlock camera;
        private IMyTextPanel lcd;
        private bool firstrun = true;
        private MyDetectedEntityInfo info;
        private StringBuilder sb = new StringBuilder();

        IMyCockpit cockpit;

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
        public Program()
        {

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            Matrix ThrLocM = new Matrix();
            Matrix MainLocM = new Matrix();
            cockpit.Orientation.GetMatrix(out MainLocM);

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

            GridTerminalSystem.GetBlocksOfType<IMyThrust>(AllThrusters);

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
        public void Main(string argument)
        {
            if (firstrun)
            {
                firstrun = false;
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocks(blocks);

                foreach (var block in blocks)
                {
                    if (block is IMyCameraBlock)
                        camera = (IMyCameraBlock)block;

                    if (block is IMyTextPanel)
                        lcd = (IMyTextPanel)block;
                }

                camera.EnableRaycast = true;
            }

            if (camera.CanScan(SCAN_DISTANCE))
            {
                info = camera.Raycast(SCAN_DISTANCE, PITCH, YAW);
                foreach (IMyThrust thr in ForwardThrusters)
                {
                    thr.ThrustOverride = (((float)Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value)) * 0.0001f * thr.MaxThrust);
                }
                foreach (IMyThrust thr in BackwardThrusters)
                {
                    thr.ThrustOverride = ( 0.0001f * thr.MaxThrust)/ ((float)Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value));
                }
            }
                

        }

        


        public void Save()
        { }
        //------------END--------------
    }
}