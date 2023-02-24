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
namespace IndustryProjector
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        static Program myScript;
        //string mainAssemblerName = "mainAssembler";
        //string projectorName = "Projector";
        //string industryScreen = "IndustryScreen";
        IndustryProjector ip;

        public Program()
        {
            myScript = this;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            ip = new IndustryProjector();
        }

        public void Main(string argument)
        {
            ip.IndustryQueue();
            //switch (argument)
            //{
            //    case "Start": 
            //        {
            //            ip.IndustryQueue();
            //            break;
            //        }
            //    case "Clear":
            //        {
            //            ip.IndustryQueueClear();
            //            break;
            //        }
            //    default: 
            //        {
            //            Runtime.UpdateFrequency = UpdateFrequency.None;
            //            break; 
            //        }
            //}
        }


        public class IndustryProjector
        {
            string mainAssemblerName = "mainAssembler";
            string projectorName = "Projector";
            string industryScreen = "IndustryScreen";


            IMyProductionBlock mainAssembler;

            IMyProjector projector;

            IMyTextPanel indMonitor;

            public IndustryProjector()
            {

                indMonitor = myScript.GridTerminalSystem.GetBlockWithName(industryScreen) as IMyTextPanel;

                mainAssembler = myScript.GridTerminalSystem.GetBlockWithName(mainAssemblerName) as IMyProductionBlock;

                projector = myScript.GridTerminalSystem.GetBlockWithName(projectorName) as IMyProjector;
            }

            //public IndustryProjector(string assembName, string projName, string scName)
            //{

            //    indMonitor = myScript.GridTerminalSystem.GetBlockWithName(scName) as IMyTextPanel;

            //    mainAssembler = myScript.GridTerminalSystem.GetBlockWithName(assembName) as IMyProductionBlock;

            //    projector = myScript.GridTerminalSystem.GetBlockWithName(projName) as IMyProjector;
            //}

            public void IndustryQueue()
            {

                if (projector.IsProjecting)
                {
                    indMonitor.WriteText("Components required:" + "\n", false);

                    foreach (var item in projector.RemainingBlocksPerType)
                    {

                        indMonitor.WriteText(item.Key.GetType().Name + " " + item.Value + "\n", true);

                    }


                }
                else indMonitor.WriteText("NONE:" + "\n", false);

            }
            public void IndustryQueueClear()
            {
                mainAssembler.ClearQueue();
            }

        }
        public void Save()
        { }

        //------------END--------------
    }
}