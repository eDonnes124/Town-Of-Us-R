using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.VenererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Ability
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Venerer))
            {
                var venerer = (Venerer) role;
                if (venerer.IsCamouflaged)
                    venerer.Ability();
                else if (venerer.Enabled) venerer.StopAbility();
            }
        }
    }
}