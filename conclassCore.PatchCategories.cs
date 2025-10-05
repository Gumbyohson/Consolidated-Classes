namespace test;

public partial class testCore 
{
    internal const string ExamplePatchCategory = "examplePatchCategory";
}

public static class PatchExtensions
{
    public static void PatchIfEnabled(this string patchCategory, bool configFlag)
    {
        if (!configFlag) return;
        testCore.HarmonyInstance.PatchCategory(patchCategory);
        testCore.Logger.VerboseDebug("Patched {0}...", patchCategory);
    }
}