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
namespace SimpleRaycastScript
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------
        //List<IMyCameraBlock> cameras;
        //MyDetectedEntityType entityType;
        double SCAN_DISTANCE = 10000;
        float PITCH = 0;
        float YAW = 0;
        private IMyCameraBlock camera;
        private IMyTextPanel lcd;
        private bool firstrun = true;
        private MyDetectedEntityInfo info;
        private StringBuilder sb = new StringBuilder();


        public Program()
        {
            //cameras = new List<IMyCameraBlock>();
            //GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(cameras);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

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
                info = camera.Raycast(SCAN_DISTANCE, PITCH, YAW);

            sb.Clear();
            sb.Append("EntityID: " + info.EntityId);
            sb.AppendLine();
            sb.Append("Name: " + info.Name);
            sb.AppendLine();
            sb.Append("Type: " + info.Type);
            sb.AppendLine();
            sb.Append("Velocity: " + info.Velocity.ToString("0.000"));
            sb.AppendLine();
            sb.Append("Relationship: " + info.Relationship);
            sb.AppendLine();
            sb.Append("Size: " + info.BoundingBox.Size.ToString("0.000"));
            sb.AppendLine();
            sb.Append("Position: " + info.Position.ToString("0.000"));

            if (info.HitPosition.HasValue)
            {
                sb.AppendLine();
                sb.Append("Hit: " + info.HitPosition.Value.ToString("0.000"));
                sb.AppendLine();
                sb.Append("Distance: " + Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value).ToString("0.00"));
            }

            sb.AppendLine();
            sb.Append("Range: " + camera.AvailableScanRange.ToString());
            lcd.WriteText(sb.ToString());
            //lcd.ShowPrivateTextOnScreen();
            //lcd.ShowPublicTextOnScreen();
        }

        public void Save()
        { }
        //------------END--------------
    }
}