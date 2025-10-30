using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using HarmonyLib;
using Vintagestory;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;
using conclass.EntityBehaviors;
using conclass.Config;
using conclass.Util;

namespace conclass
{
    public class ConclassModSystem : ModSystem {

        public static Harmony harmony;
        public static ICoreAPI Api;
        public static ICoreClientAPI CApi;
        public static ICoreServerAPI SApi;
        public static ILogger Logger;
        public static string ModID;
        public static ConclassClientConfigs Config;

        public override void StartPre(ICoreAPI api)
        {
            Api = api;
            Logger = Mod.Logger;
            ModID = Mod.Info.ModID;
            LoadConfig(api);
            
            // Set the world config values for patch conditions
            // This must be done in StartPre before patches are applied
            api.World.Config.SetBool("EnableClassStaticAssignment", Config?.EnableClassStaticAssignment ?? true);
            Logger.Notification($"Set world config 'EnableClassStaticAssignment' to: {Config?.EnableClassStaticAssignment ?? true}");
            
            // Enable temporal stability system by default
            api.World.Config.SetBool("temporalStability", true);
            Logger.Notification("Set world config 'temporalStability' to: true");
        }

        private void LoadConfig(ICoreAPI api)
        {
            try
            {
                // Try to load existing config
                Config = api.LoadModConfig<ConclassClientConfigs>("conclassConfig.json");
                
                // If no config exists, create a new one
                if (Config == null)
                {
                    Config = new ConclassClientConfigs();
                    api.StoreModConfig(Config, "conclassConfig.json");
                    Logger.Notification("Created new configuration file with default values.");
                }
                else
                {
                    // Config exists, save it to ensure any new properties are added
                    api.StoreModConfig(Config, "conclassConfig.json");
                }

                Logger.Notification("Configuration loaded successfully");
            }
            catch (Exception ex)
            {
                // If there's an error, create a default config
                Config = new ConclassClientConfigs();
                api.StoreModConfig(Config, "conclassConfig.json");
                Logger.Error("Error loading config, using defaults. Error: " + ex);
            }
        }

        public override void Start(ICoreAPI api)
        {
            try
            {
                Mod.Logger.Notification("Consolidated Classes mod is initializing...");
                
                // Register the temporal stability behavior
                api.RegisterEntityBehaviorClass("EntityBehaviorTemporalTraits", typeof(TemporalStabilityTraitBehavior));
                Mod.Logger.Notification("Temporal Stability System: ENABLED");

                // Apply patches based on configuration
                ApplyPatches();

                // Log final configuration
                Mod.Logger.Notification(Config?.ToString() ?? "No configuration loaded");
                
                // Log the current state of the condition
                Mod.Logger.Notification($"Class Static Assignment is {(Config?.EnableClassStaticAssignment == true ? "ENABLED" : "DISABLED")}");
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Failed to initialize Consolidated Classes mod:");
                Mod.Logger.Error(ex);
                throw;
            }
        }

        public override void AssetsLoaded(ICoreAPI api)
        {

        }

        private void ApplyPatches()
        {
            if (harmony != null)
            {
                return;
            }

            harmony = new Harmony(ModID);
            Logger.VerboseDebug("Initializing Harmony patches...");
            
            // The actual patching is done through JSON patches with conditions
            // The condition "EnableClassStaticAssignment" is evaluated by the game
            // based on our configuration
            
            // We can add any Harmony patches here if needed in the future
            // For example:
            // if (Config.EnableClassStaticAssignment)
            // {
            //     var original = typeof(SomeClass).GetMethod("SomeMethod");
            //     var prefix = typeof(MyPatches).GetMethod("SomeMethod_Prefix");
            //     harmony.Patch(original, new HarmonyMethod(prefix));
            // }
            
            Logger.Notification("Harmony patches initialized");
        }

        private static void HarmonyUnpatch()
        {
            Logger?.VerboseDebug("Unpatching Harmony Patches.");
            harmony?.UnpatchAll(ModID);
            harmony = null;
        }

        public override void Dispose()
        {
            HarmonyUnpatch();
            Logger = null;
            ModID = null;
            Api = null;
            base.Dispose();
        }

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void StartServerSide(ICoreServerAPI api)
        {
            Mod.Logger.Notification("Consolidated classes mod loaded server side: " + Lang.Get("conclass:hello"));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            Mod.Logger.Notification("Consolidated classes mod loaded client side: " + Lang.Get("conclass:hello"));
        }

    }
}
