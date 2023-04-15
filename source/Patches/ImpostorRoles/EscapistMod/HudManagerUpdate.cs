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


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Escapist)) return;
            var role = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);
            var button = __instance.AbilityButton;

            if (button.graphic.sprite != MarkSprite && button.graphic.sprite != EscapeSprite)
                button.graphic.sprite = MarkSprite;

   
            button.graphic.color = Palette.EnabledColor;
            button.graphic.material.SetFloat("_Desat", 0f);
            if (button.graphic.sprite == MarkSprite) button.SetCoolDown(0f, 1f);
            else button.SetCoolDown(role.EscapeTimer(), CustomGameOptions.EscapeCd);
        }
    }
}
