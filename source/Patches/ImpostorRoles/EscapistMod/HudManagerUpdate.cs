using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.EscapistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite MarkSprite => TownOfUs.MarkSprite;
        public static Sprite EscapeSprite => TownOfUs.EscapeSprite;


        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Escapist)) return;
            var role = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);

            if (role.RoleAbilityButton.graphic.sprite != MarkSprite && role.RoleAbilityButton.graphic.sprite != EscapeSprite)
                role.RoleAbilityButton.graphic.sprite = MarkSprite;

            role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
            role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
            if (role.RoleAbilityButton.graphic.sprite == MarkSprite) role.RoleAbilityButton.SetCoolDown(0f, 1f);
            else role.RoleAbilityButton.SetCoolDown(role.EscapeTimer(), CustomGameOptions.EscapeCd);
        }
    }
}
