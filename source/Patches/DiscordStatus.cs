using Discord;
using HarmonyLib;
namespace TownOfUs.Patches
{
    [HarmonyPatch]
    internal class DiscordStatus
    {
        [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
        [HarmonyPrefix]
        public static void Prefix([HarmonyArgument(0)] Activity activity)
        {
            activity.Details += $" Town of Us v{TownOfUs.VersionString}";
        }
    }
}
