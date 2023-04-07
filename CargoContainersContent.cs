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
namespace CargoContainersContent
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------


        static Program myScript;
        Cargo c1;
        uint tickCount = 0;
        uint timePassed = 0;

        public Program()
        {
            myScript = this;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            c1 = new Cargo();
        }

        public void Main(string args)
        {
            c1.ShowItems();
            tickCount = tickCount + 1;
            timePassed = tickCount * (100 / 60);
            c1.ToolsLCD.WriteText("\n" + "Time Passed: " + timePassed + " s" + "\n", true);
        }

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
            public IMyTextPanel ToolsLCD;

            List<MyInventoryItem> ContainerItems;
            List<IMyTerminalBlock> Blocks;
            List<IMyInventory> ContainersInventories;
            StringBuilder sb = new StringBuilder();

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


            public StringBuilder ProgressBar(MyFixedPoint curItemCount)
            {
                int maxSymbolCount = 50;
                float maxItemCount = 20000f;
                sb.Clear();
                int curSymbolCount = 0;
                int curDotCount = 0;
                if ((float)curItemCount <= maxItemCount)
                {
                    curSymbolCount = (int)(50 * ((float)curItemCount / maxItemCount));
                    curDotCount = maxSymbolCount - curSymbolCount;
                }
                else
                {
                    curSymbolCount = maxSymbolCount;
                    curDotCount = 0;
                }
                if (curSymbolCount + curDotCount < maxSymbolCount)
                {
                    curSymbolCount = curSymbolCount + (maxSymbolCount - curDotCount - curSymbolCount);
                } 
                sb.Append('[');
                for (int i = 0; i < curSymbolCount; i++)
                {
                    sb.Append('I');
                }
                for (int j = 0; j < curDotCount; j++)
                {
                    sb.Append('.');
                }
                sb.Append("]");
                return sb;
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
                    if (Block.HasInventory & Block.InventoryCount < 2)
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
                    //if (item.Type.TypeId == Ores) OresLCD.WriteText(item.Type.SubtypeId + " " + Math.Round((decimal)Amount) + "\n", true);
                    //else if (item.Type.TypeId == Ingots)
                    //{
                    //    if (item.Type.SubtypeId == "Thorium") IngotsLCD.WriteText(item.Type.SubtypeId + " " + Math.Round((decimal)Amount, 2) + "\n", true);
                    //    else IngotsLCD.WriteText(item.Type.SubtypeId + " " + Math.Round((decimal)Amount) + "\n", true);
                    //} 
                    //else if (item.Type.TypeId == Components) ComponentsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                    //else if (item.Type.TypeId == OxygenContainerObject) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                    //else if (item.Type.TypeId == ConsumableItem) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                    //else if (item.Type.TypeId == AmmoMagazine) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                    //else if (item.Type.TypeId == PhysicalGunObject) ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                    //Amount = 0;
                    if (item.Type.TypeId == Ores)
                    {
                        OresLCD.WriteText(item.Type.SubtypeId + " " + Math.Round((decimal)Amount) + "\n", true);
                        OresLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                    }
                    else if (item.Type.TypeId == Ingots)
                    {
                        if (item.Type.SubtypeId == "Thorium")
                        {
                            IngotsLCD.WriteText(item.Type.SubtypeId + " " + Math.Round((decimal)Amount, 2) + "\n", true);
                            IngotsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                        }
                        else
                        {
                            IngotsLCD.WriteText(item.Type.SubtypeId + " " + Math.Round((decimal)Amount) + "\n", true);
                            IngotsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                        }

                    }
                    else if (item.Type.TypeId == Components)
                    {
                        ComponentsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                        //ComponentsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                    }
                    else if (item.Type.TypeId == OxygenContainerObject)
                    {
                        ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                        //ToolsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                    }

                    else if (item.Type.TypeId == ConsumableItem)
                    {
                        ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                        //ToolsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                    }


                    else if (item.Type.TypeId == AmmoMagazine)
                    {
                        ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                        //ToolsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                    }
                    else if (item.Type.TypeId == PhysicalGunObject) 
                    {
                        ToolsLCD.WriteText(item.Type.SubtypeId + " " + Amount + "\n", true);
                        //ToolsLCD.WriteText(ProgressBar(Amount).ToString() + "\n", true);
                    } 
                    Amount = 0;
                }

                ContainerItems.Clear();
                ContainersInventories.Clear();
                sb.Clear();
            }

        }

        public void Save()
        { }

        //------------END--------------
    }
}