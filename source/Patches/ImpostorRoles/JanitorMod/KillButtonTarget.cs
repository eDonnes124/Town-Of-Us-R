using HarmonyLib;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Janitor)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }
    }
}