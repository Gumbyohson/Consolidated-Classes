using test.Config;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace test;
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public partial class testCore : ModSystem
{
    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static Harmony HarmonyInstance { get; private set; }
    public static ModConfig Config => ConfigLoader.Config;

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        ModId = Mod.Info.ModID;
        Patch();
    }
    
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        Logger.Notification("Hello from template mod: " + api.Side);
        Logger.StoryEvent("Templates lost..."); // Sample story event (shown when loading a world)
        Logger.Event("Templates loaded..."); // Sample event (shown when loading in dev mode or in logs)
    }
    
    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        Logger.Notification("Hello from template mod client side: " + Lang.Get("test:hello"));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        Logger.Notification("Hello from template mod server side: " + Lang.Get("test:hello"));
    }

    public static void Patch()
    {
        if (HarmonyInstance != null) return;
        HarmonyInstance = new Harmony(ModId);
        Logger.VerboseDebug("Patching...");
        ExamplePatchCategory.PatchIfEnabled(Config.ExampleConfigSetting);
    }

    public static void Unpatch()
    {
        Logger?.VerboseDebug("Unpatching...");
        HarmonyInstance?.UnpatchAll();
        HarmonyInstance = null;
    }
    
    public override void Dispose()
    {
        Unpatch();
        Logger = null;
        ModId = null;
        Api = null;
        base.Dispose();
    }
}
