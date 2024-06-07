using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.MercenaryMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ArmorUnarmor
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Mercenary))
            {
                var merc = (Mercenary) role;
                if (merc.Armored)
                    merc.DonArmor();
                else if (merc.Enabled) merc.RemoveArmor();
            }
        }
    }
}