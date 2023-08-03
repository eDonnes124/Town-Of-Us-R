using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.GrenadierMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class FlashUnFlash
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier) role;
                if (grenadier.Flashed)
                    grenadier.Flash();
                else if (grenadier.Enabled) grenadier.UnFlash();
            }
        }
    }
}