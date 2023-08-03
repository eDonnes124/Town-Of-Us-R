using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective)) return true;
            else
            {
                var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                if (__instance == detective.RoleAbilityButton) return true;
                else return false;
            }
        }
    }
}