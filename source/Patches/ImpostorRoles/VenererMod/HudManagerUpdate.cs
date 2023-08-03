using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.VenererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite NoneSprite => TownOfUs.NoAbilitySprite;
        public static Sprite CamoSprite => TownOfUs.CamouflageSprite;
        public static Sprite CamoSprintSprite => TownOfUs.CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite => TownOfUs.CamoSprintFreezeSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Venerer)) return;
            var role = Role.GetRole<Venerer>(PlayerControl.LocalPlayer);

            if (role.Kills == 0) role.RoleAbilityButton.graphic.sprite = NoneSprite;
            else if (role.Kills == 1) role.RoleAbilityButton.graphic.sprite = CamoSprite;
            else if (role.Kills == 2) role.RoleAbilityButton.graphic.sprite = CamoSprintSprite;
            else role.RoleAbilityButton.graphic.sprite = CamoSprintFreezeSprite;

            if (role.IsCamouflaged)
            {
                role.RoleAbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AbilityDuration);
                return;
            }

            if (role.Kills > 0)
            {
                role.RoleAbilityButton.SetCoolDown(role.AbilityTimer(), CustomGameOptions.AbilityCd);
                role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.RoleAbilityButton.SetCoolDown(0, CustomGameOptions.AbilityCd);
                role.RoleAbilityButton.graphic.color = Palette.DisabledClear;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 1f);
            }

        }
    }
}