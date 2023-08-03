using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SampleSprite => TownOfUs.SampleSprite;
        public static Sprite MorphSprite => TownOfUs.MorphSprite;


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) return;
            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (role.RoleAbilityButton.graphic.sprite != SampleSprite && role.RoleAbilityButton.graphic.sprite != MorphSprite)
                role.RoleAbilityButton.graphic.sprite = SampleSprite;

            role.RoleAbilityButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.RoleAbilityButton.graphic.sprite == SampleSprite)
            {
                role.RoleAbilityButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.RoleAbilityButton);
            }
            else
            {
                if (role.Morphed)
                {
                    role.RoleAbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                role.RoleAbilityButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
