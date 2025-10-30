using Vintagestory.API.Common;
using conclass.Config;

namespace conclass.Util
{
    public static class ClassStaticAssignmentConfig
    {
        public static bool IsEnabled(ICoreAPI api)
        {
            try
            {
                // Access the static Config field directly
                if (ConclassModSystem.Config != null)
                {
                    return ConclassModSystem.Config.EnableClassStaticAssignment;
                }
                
                // If we get here, the config isn't loaded yet
                // Try to get the mod system and force config loading
                var modSystem = api.ModLoader.GetModSystem<ConclassModSystem>();
                if (modSystem == null) return false;
                
                // The config should now be loaded, try to access it again
                return ConclassModSystem.Config?.EnableClassStaticAssignment ?? false;
            }
            catch
            {
                // If there's any error, default to false for safety
                return false;
            }
        }
    }
}
