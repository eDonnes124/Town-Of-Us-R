using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class StalkUnstalk
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Hunter))
            {
                var hunter = (Hunter) role;
                if (hunter.Stalking)
                    hunter.Stalk();
                else if (hunter.Enabled) hunter.StopStalking();
            }
        }
    }
}