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

namespace GravDriveWithDampeners
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        string GravDriveGroupName = "GravDrive1";
        string ControllerName = "FlightSeat";
        static Program myScript;
        GravDrive gd1;

        public Program()
        {
            myScript = this;
            gd1 = new GravDrive(GravDriveGroupName, ControllerName);
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main()
        {
            gd1.Update();
        }

        public class GravDrive
        {
            private float dampK = 0.1f;
            private IMyShipController Control;
            private List<IMyArtificialMassBlock> MassBlocks;
            private List<IMyGravityGenerator> GravGens;
            private IMyBlockGroup myGroup;
            //private bool gdOverride;

            public GravDrive(string groupName, string contrName)
            {
                // находим блоки
                Control = myScript.GridTerminalSystem.GetBlockWithName(contrName) as IMyShipController;
                myGroup = myScript.GridTerminalSystem.GetBlockGroupWithName(groupName);
                MassBlocks = new List<IMyArtificialMassBlock>();
                GravGens = new List<IMyGravityGenerator>();
                myGroup.GetBlocksOfType<IMyArtificialMassBlock>(MassBlocks);
                myGroup.GetBlocksOfType<IMyGravityGenerator>(GravGens);
                //Control.ControlThrusters = true;
            }

            public void Update()
            {
                
                Vector3D myVelocity = Control.GetShipVelocities().LinearVelocity;
                Vector3D PilotInput = Control.MoveIndicator;
                
                bool gdOverride = Control.DampenersOverride;
                if (gdOverride)
                {
                    if (Math.Abs(PilotInput.X) < 0.00001)
                    {
                        PilotInput.X = -myVelocity.Dot(Control.WorldMatrix.Right) * dampK;
                    }
                    if (Math.Abs(PilotInput.Y) < 0.00001)
                    {
                        PilotInput.Y = -myVelocity.Dot(Control.WorldMatrix.Up) * dampK;
                    }
                    if (Math.Abs(PilotInput.Z) < 0.00001)
                    {
                        PilotInput.Z = -myVelocity.Dot(Control.WorldMatrix.Backward) * dampK;
                    }
                }
                Vector3D PilotInputW = Vector3D.Transform(PilotInput, Control.WorldMatrix.GetOrientation());

                foreach (IMyGravityGenerator gravgen in GravGens)
                {
                    gravgen.GravityAcceleration = (float)PilotInputW.Dot(gravgen.WorldMatrix.Down) * 9.8f;
                }
            }

        }
        //------------END--------------
    }
}
