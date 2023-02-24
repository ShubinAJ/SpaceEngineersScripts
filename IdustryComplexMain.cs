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
using VRage;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Text.RegularExpressions;

namespace SE_Script
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------


        static Program myScript;
        Cargo c1;
        IndustryProjector ip;
        //static string Ores = "MyObjectBuilder_Ore";
        //static string Ingots = "MyObjectBuilder_Ingot";
        //static string Components = "MyObjectBuilder_Component";
        //static string OxygenContainerObject = "MyObjectBuilder_OxygenContainerObject";
        //static string ConsumableItem = "MyObjectBuilder_ConsumableItem";
        //static string AmmoMagazine = "MyObjectBuilder_AmmoMagazine";
        //static string PhysicalGunObject = "MyObjectBuilder_PhysicalGunObject";

        //string lcdNameOres = "OresLCD";
        //string lcdNameIngots = "IngotsLCD";
        //string lcdNameComponents = "ComponentsLCD";
        //string lcdNameToolsAmmo = "ToolsLCD";

        public Program()
        {
            myScript = this;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            //c1 = new Cargo(lcdNameOres, lcdNameIngots, lcdNameComponents, lcdNameToolsAmmo);
            c1 = new Cargo();
            ip = new IndustryProjector();
        }

        public void Main(string args)
        {
            c1.ShowItems();
            ip.IndustryQueue();
        }

        //---------------------------------------

        public class IndustryProjector
        {
            //string mainAssemblerName = "mainAssembler";
            string projectorName = "Projector";
            string industryScreen = "IndustryScreen";


            //IMyProductionBlock mainAssembler;

            IMyProjector projector;

            IMyTextPanel indMonitor;

            public IndustryProjector()
            {

                indMonitor = myScript.GridTerminalSystem.GetBlockWithName(industryScreen) as IMyTextPanel;

                //mainAssembler = myScript.GridTerminalSystem.GetBlockWithName(mainAssemblerName) as IMyProductionBlock;

                projector = myScript.GridTerminalSystem.GetBlockWithName(projectorName) as IMyProjector;
            }



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
            //public void IndustryQueueClear()
            //{
            //    mainAssembler.ClearQueue();
            //}

        }

        //---------------------------------------



        public class Cargo
        {

            static string Ores = "MyObjectBuilder_Ore";
            static string Ingots = "MyObjectBuilder_Ingot";
            static string Components = "MyObjectBuilder_Component";
            static string OxygenContainerObject = "MyObjectBuilder_OxygenContainerObject";
            static string ConsumableItem = "MyObjectBuilder_ConsumableItem";
            static string AmmoMagazine = "MyObjectBuilder_AmmoMagazine";
            static string PhysicalGunObject = "MyObjectBuilder_PhysicalGunObject";

            string lcdNameOres = "OresLCD";
            string lcdNameIngots = "IngotsLCD";
            string lcdNameComponents = "ComponentsLCD";
            string lcdNameToolsAmmo = "ToolsLCD";

            IMyTextPanel OresLCD;
            IMyTextPanel IngotsLCD;
            IMyTextPanel ComponentsLCD;
            IMyTextPanel ToolsLCD;

            List<MyInventoryItem> ContainerItems;
            List<IMyTerminalBlock> Blocks;
            List<IMyInventory> ContainersInventories;


            public Cargo()
            {
                OresLCD = myScript.GridTerminalSystem.GetBlockWithName(lcdNameOres) as IMyTextPanel;
                IngotsLCD = myScript.GridTerminalSystem.GetBlockWithName(lcdNameIngots) as IMyTextPanel;
                ComponentsLCD = myScript.GridTerminalSystem.GetBlockWithName(lcdNameComponents) as IMyTextPanel;
                ToolsLCD = myScript.GridTerminalSystem.GetBlockWithName(lcdNameToolsAmmo) as IMyTextPanel;
                Blocks = new List<IMyTerminalBlock>();
                myScript.GridTerminalSystem.GetBlocks(Blocks);
                ContainerItems = new List<MyInventoryItem>();
                ContainersInventories = new List<IMyInventory>();
            }

            public void ShowItems()
            {
                IMyInventory ContainerInventory;
                MyFixedPoint Amount = 0;

                OresLCD.WriteText("Ores:" + "\n", false);
                IngotsLCD.WriteText("Ingots:" + "\n", false);
                ComponentsLCD.WriteText("Components:" + "\n", false);
                ToolsLCD.WriteText("Tools:" + "\n", false);

                foreach (IMyTerminalBlock Block in Blocks)
                {

                    if (Block.HasInventory & Block.InventoryCount > 1)
                    {

                        for (int i = 0; i < Block.InventoryCount; i++)
                        {
                            ContainerInventory = Block.GetInventory(i);
                            ContainersInventories.Add(ContainerInventory);
                            ContainerInventory.GetItems(ContainerItems);
                        }
                    }

                    if (Block.HasInventory)
                    {
                        ContainerInventory = Block.GetInventory();
                        ContainersInventories.Add(ContainerInventory);
                        ContainerInventory.GetItems(ContainerItems);
                    }
                }

                ContainerItems = ContainerItems.GroupBy(x => x.Type).Select(x => x.First()).ToList();

                foreach (MyInventoryItem item in ContainerItems)
                {
                    foreach (IMyInventory inventory in ContainersInventories)
                    {
                        Amount += inventory.GetItemAmount(item.Type);
                    }
                    if (item.Type.TypeId == Ores) OresLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    else if (item.Type.TypeId == Ingots) IngotsLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    else if (item.Type.TypeId == Components) ComponentsLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    else if (item.Type.TypeId == OxygenContainerObject) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    else if (item.Type.TypeId == ConsumableItem) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    else if (item.Type.TypeId == AmmoMagazine) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    else if (item.Type.TypeId == PhysicalGunObject) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount.ToIntSafe() + "\n", true);
                    Amount = 0;
                }
                ContainerItems.Clear();
                ContainersInventories.Clear();

            }

        }

        //----------------------------

        public void Save()
        { }

        //------------END--------------
    }

}
