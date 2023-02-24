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
namespace AutodoorsAndOxGens
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------


        List<IMyGasTank> oxygenTanks;
        List<IMyGasGenerator> oxygenGenerators;
        List<MyDetectedEntityInfo> entity_list;
        List<IMyAirVent> vents;
        //IMyTextPanel LCD;

        public Program()
        {
            oxygenTanks = new List<IMyGasTank>();
            oxygenGenerators = new List<IMyGasGenerator>();
            vents = new List<IMyAirVent>();
            entity_list = new List<MyDetectedEntityInfo>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(oxygenTanks);
            GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(oxygenGenerators);
            GridTerminalSystem.GetBlocksOfType<IMyAirVent>(vents);
            //LCD = (IMyTextPanel) GridTerminalSystem.GetBlockWithName("LCD");
            //LCD.ContentType = ContentType.TEXT_AND_IMAGE;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string args)
        {
            // управление кислородными генераторами
            foreach (IMyGasTank tank in oxygenTanks)
            {
                if (tank.FilledRatio > 0.9)
                {
                    foreach (IMyGasGenerator gen in oxygenGenerators)
                    {
                        gen.ApplyAction("OnOff_Off");
                    }
                }
                else
                {
                    foreach (IMyGasGenerator gen in oxygenGenerators)
                    {
                        gen.ApplyAction("OnOff_On");
                    }
                }
            }

            // управление внешними дверьми и герметизация

            var door_sensor_right = GridTerminalSystem.GetBlockWithName("Sensor Right") as IMySensorBlock;
            door_sensor_right.DetectedEntities(entity_list);
            if (entity_list.Count == 0)
            {
                var door = GridTerminalSystem.GetBlockWithName("Outer Door Right") as IMyDoor;
                door.ApplyAction("Open_Off");
                foreach (var vent in vents) vent.Depressurize = false;
            }
            else
            {
                var door = GridTerminalSystem.GetBlockWithName("Outer Door Right") as IMyDoor;
                foreach (var vent in vents) vent.Depressurize = true;
                if (vents[0].GetOxygenLevel() == 0 & vents[1].GetOxygenLevel() == 0) door.ApplyAction("Open_On");
            }
            var door_sensor_left = GridTerminalSystem.GetBlockWithName("Sensor Left") as IMySensorBlock;
            door_sensor_left.DetectedEntities(entity_list);
            if (entity_list.Count == 0)
            {
                var door = GridTerminalSystem.GetBlockWithName("Outer Door Left") as IMyDoor;
                door.ApplyAction("Open_Off");
            }
            else
            {
                var door = GridTerminalSystem.GetBlockWithName("Outer Door Left") as IMyDoor;
                foreach (var vent in vents) vent.Depressurize = true;
                if (vents[0].GetOxygenLevel() == 0 & vents[1].GetOxygenLevel() == 0) door.ApplyAction("Open_On");
            }

            //foreach (IMySensorBlock sensor in sensors)
            //{
            //    if (sensor.IsActive)
            //    {
            //        LCD.WriteText("Sensor Name:" + sensor.Name.ToString() + "\n", true);
            //    }
            //}
        }

        public void Save()
        { }

        //------------END--------------
    }
}