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
            if (role.AbilityButton == null)
            {
                role.AbilityButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.AbilityButton.graphic.enabled = true;
                role.AbilityButton.gameObject.SetActive(false);
            }
            if (role.Kills == 0) role.AbilityButton.graphic.sprite = NoneSprite;
            else if (role.Kills == 1) role.AbilityButton.graphic.sprite = CamoSprite;
            else if (role.Kills == 2) role.AbilityButton.graphic.sprite = CamoSprintSprite;
            else role.AbilityButton.graphic.sprite = CamoSprintFreezeSprite;
            role.AbilityButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.IsCamouflaged)
            {
                role.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AbilityDuration);
                return;
            }

            if (role.Kills > 0)
            {
                role.AbilityButton.SetCoolDown(role.AbilityTimer(), CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.EnabledColor;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.AbilityButton.SetCoolDown(0, CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.DisabledClear;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
            }

        }
    }
}