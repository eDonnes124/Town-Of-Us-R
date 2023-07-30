using HarmonyLib;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }
    }
}