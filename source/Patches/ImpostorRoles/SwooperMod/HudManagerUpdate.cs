using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.SwooperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SwoopSprite => TownOfUs.SwoopSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swooper)) return;
            var role = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);

            role.RoleAbilityButton.graphic.sprite = SwoopSprite;

            if (role.IsSwooped)
            {
                role.RoleAbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
                return;
            }

            role.RoleAbilityButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCd);


            role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
            role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}