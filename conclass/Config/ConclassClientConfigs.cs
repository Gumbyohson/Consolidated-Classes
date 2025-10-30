using System;
using System.ComponentModel;
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

        [Description("Enable the Temporal Stability system. When disabled, all temporal stability effects will be turned off.")]
        public bool EnableTemporalStability { get; set; } = true;

        public override string ToString()
        {
            return "Conclass Configuration:\n" +
                   $"  • Class Static Assignment: {EnableClassStaticAssignment.ToYesNo()}\n" +
                   $"  • Temporal Stability System: {EnableTemporalStability.ToYesNo()}";
        }
    }
}
