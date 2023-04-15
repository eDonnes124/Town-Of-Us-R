using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class SetTarget
    {
        public static void Postfix(AbilityButton __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) return;
            if (target != null)
                if (target.Data.IsImpostor())
                {
                    __instance.graphic.color = Palette.DisabledClear;
                    __instance.graphic.material.SetFloat("_Desat", 1f);
                }
        }
    }
}