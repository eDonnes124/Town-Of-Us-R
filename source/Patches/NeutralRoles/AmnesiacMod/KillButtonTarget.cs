using HarmonyLib;

namespace TownOfUs.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix()
        {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);
        }
    }
}