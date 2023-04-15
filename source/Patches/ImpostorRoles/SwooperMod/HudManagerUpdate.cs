using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.SwooperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swooper)) return;
            var role = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);

            if (role.IsSwooped)
            {
                __instance.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
                return;
            }

            __instance.AbilityButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCd);


            __instance.AbilityButton.graphic.color = Palette.EnabledColor;
            __instance.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}