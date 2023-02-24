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

namespace BaseEnergy
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        public IMyTextSurface TextPanel;
        public List<IMyBatteryBlock> Batteries;
        float TotalEnergy;
        float MaxBaseEnergy;
        public IMyReactor MainReactor;
        public Program()
        {
            TextPanel = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextSurface;
            TextPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            MainReactor = GridTerminalSystem.GetBlockWithName("Base MAIN REACTOR") as IMyReactor;
            Batteries = new List<IMyBatteryBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(Batteries);
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
             
        }
        public void Main(string argument)
        {
            TotalEnergy = 0.0f;
            MaxBaseEnergy = 0.0f;
            foreach (IMyBatteryBlock battery in Batteries)
            {
                TotalEnergy += battery.CurrentStoredPower;
                MaxBaseEnergy += battery.MaxStoredPower;
            }
            Supply();
            Show();

        }

        public void Show()
        {
            TextPanel.WriteText(TotalEnergy.ToString() + " MWh " + MainReactor.CurrentOutput + "/" + MainReactor.MaxOutput + " MWh", false);
            
        }
        public void Supply()
        {
            if ((TotalEnergy <= 0.8 * MaxBaseEnergy) && (MainReactor.GetInventory().CurrentMass >= 680))
            {
                MainReactor.ApplyAction("OnOff_On");
            }
            else MainReactor.ApplyAction("OnOff_Off");
            
        }




        public void Save()
        { }
        //------------END--------------
    }
}
