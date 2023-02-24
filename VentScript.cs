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
namespace VentScript
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------

        List <IMySensorBlock> sensors;
        List <IMyAirtightSlideDoor> doors;
        IMyAirVent vents;
        IMyGasTank oxygenTanks;
        IMyGasGenerator oxygenGenerators;
        public Program()
        {
            sensors = new List <IMySensorBlock>();
            doors = new List <IMyAirtightSlideDoor>();
            GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(sensors);
            GridTerminalSystem.GetBlocksOfType<IMyAirtightSlideDoor>(doors);
            vents = (IMyAirVent) GridTerminalSystem.GetBlockGroupWithName("Air Vents");
            oxygenTanks = (IMyGasTank) GridTerminalSystem.GetBlockGroupWithName("Oxygen Tanks");
            oxygenGenerators = (IMyGasGenerator)GridTerminalSystem.GetBlockGroupWithName("Oxygen Generators");
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string args)
        {
            if (oxygenTanks.FilledRatio >= 0.9) 
            {
                oxygenGenerators.ApplyAction("OnOff_Off");
            }
            else oxygenGenerators.ApplyAction("OnOff_On");
           foreach (IMySensorBlock currentSensor in sensors)
            {
                if (currentSensor.Name == "Sensor Left")
                {
                    //if ((currentSensor.IsActive) && (currentSensor.HasLocalPlayerAccess()))
                    if (currentSensor.IsActive)
                    {
                        //vents.Depressurize = true;

                        //if (vents.GetOxygenLevel() == 0)
                        //{
                            foreach (var door in doors)
                            {
                                if (door.Name == "Door Left") door.OpenDoor();
                            }
                        //}
                    }
                    //else vents.Depressurize = false;
                }
                else if (currentSensor.Name == "Sensor Right")
                {
                    //if ((currentSensor.IsActive) && (currentSensor.HasLocalPlayerAccess()))
                    if (currentSensor.IsActive)
                    {
                        //vents.Depressurize = true;
                        //if (vents.GetOxygenLevel() == 0)
                        //{
                            foreach (var door in doors)
                            {
                                if (door.Name == "Door RIght") door.OpenDoor();
                            }
                        //}
                    }
                    //else vents.Depressurize = false;
                }
            }
            
        }

        public void Save()
        { }

        //------------END--------------
    }
}