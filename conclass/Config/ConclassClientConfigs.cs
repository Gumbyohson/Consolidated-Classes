using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Vintagestory.API.Config;

namespace conclass.Config
{
    public static class ConfigExtensions
    {
        public static string ToYesNo(this bool value) => value ? "Enabled" : "Disabled";
    }

    public class ConclassClientConfigs
    {
        [Description("Enable class static assignment features. This controls both Player Model Library and Racial Equality patches.")]
        public bool EnableClassStaticAssignment { get; set; } = true;

        [Description("Enable Player Model Library patching. When enabled, this will modify the player model system.")]
        public bool EnablePlayerModelLibPatch 
        { 
            get => EnableClassStaticAssignment;
            set => EnableClassStaticAssignment = value;
        }

        [Description("Enable Racial Equality features. When enabled, this will apply racial equality settings.")]
        public bool EnableRacialEquality 
        { 
            get => EnableClassStaticAssignment;
            set => EnableClassStaticAssignment = value;
        }

        public override string ToString()
        {
            return "Conclass Configuration:\n" +
                   $"  • Class Static Assignment: {EnableClassStaticAssignment.ToYesNo()}\n" +
                   $"  • Player Model Library Patch: {EnablePlayerModelLibPatch.ToYesNo()}\n" +
                   $"  • Racial Equality: {EnableRacialEquality.ToYesNo()}";
        }
    }
}
