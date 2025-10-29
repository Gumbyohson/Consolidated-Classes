using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using conclass.src.EntityBehaviors;
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

namespace conclass.src
{
    public class conclassModSystem : ModSystem {

        public static Harmony harmony;
        public static ICoreAPI Api;
        public static ICoreClientAPI CApi;
        public static ICoreServerAPI SApi;
        public static ILogger Logger;
        public static string ModID;

        public override void StartPre(ICoreAPI api)
        {
            Api = api;
            Logger = Mod.Logger;
            ModID = Mod.Info.ModID;
        }

        public override void Start(ICoreAPI api)
        {
            api.RegisterEntityBehaviorClass("EntityBehaviorTemporalTraits", typeof(TemporalStabilityTraitBehavior));

            ApplyPatches();
        }

        public override void AssetsLoaded(ICoreAPI api)
        {

        }

        private static void ApplyPatches()
        {
            if (harmony != null)
            {
                return;
            }
            harmony = new Harmony(ModID);
            Logger.VerboseDebug("Harmony is starting Patches!");
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
        public override void Start(ICoreAPI api)
        {
            Mod.Logger.Notification("Consolidated classes mod loaded: " + api.Side);
        }

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
