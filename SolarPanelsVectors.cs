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

namespace SolarPanelsVectors
{
    public sealed class Program : MyGridProgram
    {

        //------------BEGIN--------------


        SolarEffectivity se;

        public Program()
        {

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            se = new SolarEffectivity(this);
        }

        public void Main(string argument)
        {

            switch (argument)
            {
                case "Start":
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update1;
                        se.TrackSun();
                        break;
                    }
                case "Stop":
                    {
                        se.StatorHorizontal.TargetVelocityRad = 0.0f;
                        se.HingeVertical.TargetVelocityRad = 0.0f;
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        break;
                    }
                default:
                    {
                    se.TrackSun();
                    break;
                    }       
            }
        }
        class SolarEffectivity
        {
            public IMySolarPanel Panel;
            public IMyMotorAdvancedStator HingeVertical;
            public IMyMotorStator StatorHorizontal;
            Program parentProgram;
            public SolarEffectivity(Program parProg)
            {
                parentProgram = parProg;
                Panel = parentProgram.GridTerminalSystem.GetBlockWithName("Solar Panel") as IMySolarPanel;
                StatorHorizontal = parentProgram.GridTerminalSystem.GetBlockWithName("Rotor Horizontal") as IMyMotorStator;
                HingeVertical = parentProgram.GridTerminalSystem.GetBlockWithName("Vertical Hinge") as IMyMotorAdvancedStator;
            }
            float SolarOutput = 0;
            float SolarOutputVert = 0;
            int dir = 1;
            int cnt = 0;
            int dirVert = 1;
            int cntVert = 0;

            public void TrackSun()
            {
                cnt++;
                cntVert++;

                if (cnt >= 180)
                    {
                        float OutputGain = Panel.MaxOutput - SolarOutput;
                        if (OutputGain < 0) dir *= -1;
                        SolarOutput = Panel.MaxOutput;
                        cnt = 0;
                    }
                if (cntVert >= 180)
                {
                    float OutputGainVert = Panel.MaxOutput - SolarOutputVert;
                    if (OutputGainVert < 0) dirVert *= -1;
                    SolarOutputVert = Panel.MaxOutput;
                    cntVert = 0;
                }
                float VelocityHor = (float)(dir * (150.0f - Panel.MaxOutput) * 0.0001);
                float VelocityVert = (float)(dirVert * (150.0f - Panel.MaxOutput) * 0.0001);
                if (Math.Abs(VelocityHor) < 0.03f)
                {
                    HingeVertical.TargetVelocityRad = VelocityVert;
                }
                else HingeVertical.TargetVelocityRad = 0.0f;
                StatorHorizontal.TargetVelocityRad = VelocityHor;
                
            }
        }
        //------------END--------------
    }
}
