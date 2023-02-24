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

        public Program()
        { }

        public void Main(string args)
        {

        }

        class MyThrusters
        {

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

            }
            
            private void InitMainBlocks()
            {

            }


        }


        public void Save()
        { }

        //------------END--------------
    }

}
