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
                    // Validate and reset missing or invalid values
                    var defaultConfig = new ConclassClientConfigs();
                    bool updated = false;

                    if (Config.EnableClassStaticAssignment != true && Config.EnableClassStaticAssignment != false)
                    {
                        Config.EnableClassStaticAssignment = defaultConfig.EnableClassStaticAssignment;
                        updated = true;
                    }

                    if (Config.EnableTemporalStability != true && Config.EnableTemporalStability != false)
                    {
                        Config.EnableTemporalStability = defaultConfig.EnableTemporalStability;
                        updated = true;
                    }

                    // Save the updated config if changes were made
                    if (updated)
                    {
                        api.StoreModConfig(Config, "conclassConfig.json");
                        Logger.Notification("Configuration file updated with default values for missing or invalid settings.");
                    }
                }

                Logger.Notification($"Configuration loaded successfully. EnableClassStaticAssignment: {Config.EnableClassStaticAssignment}, EnableTemporalStability: {Config.EnableTemporalStability}");
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
            SApi = api;
            
            // Server sets these values in the World Config
            api.World.Config.SetBool("EnableClassStaticAssignment", Config?.EnableClassStaticAssignment ?? true);
            api.World.Config.SetBool("EnableTemporalStability", Config?.EnableTemporalStability ?? true);
            
            Logger.Notification($"Server config initialized: ClassStaticAssignment={Config?.EnableClassStaticAssignment ?? true}, TemporalStability={Config?.EnableTemporalStability ?? true}");
            Logger.Notification($"Server temporal stability settings - Mod Config: {Config?.EnableTemporalStability ?? true}, World Config: {api.World.Config.GetBool("temporalStability", true)}");
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            CApi = api;
            
            // Client reads the values from World Config
            bool classStaticAssignment = api.World.Config.GetBool("EnableClassStaticAssignment", true);
            bool temporalStability = api.World.Config.GetBool("EnableTemporalStability", true);
            
            // Update client config to match server values
            if (Config != null)
            {
                Config.EnableClassStaticAssignment = classStaticAssignment;
                Config.EnableTemporalStability = temporalStability;
            }

            Logger.Notification($"Client config synchronized: ClassStaticAssignment={classStaticAssignment}, TemporalStability={temporalStability}");
        }

    }
}
