using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.ImpostorRoles.EclipsalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite BlindSprite => TownOfUs.BlindSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Eclipsal)) return;
            var role = Role.GetRole<Eclipsal>(PlayerControl.LocalPlayer);
            if (role.BlindButton == null)
            {
                role.BlindButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlindButton.graphic.enabled = true;
                role.BlindButton.gameObject.SetActive(false);
            }

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer && !player.Data.IsImpostor())
                    {
                        var tempColour = player.nameText().color;
                        var data = player?.Data;
                        if (data == null || data.Disconnected || data.IsDead)
                            continue;
                        if (role.BlindPlayers.Contains(player))
                        {
                            player.myRend().material.SetColor("_VisorColor", new Color(0.32f, 0.13f, 0.13f));
                            player.nameText().color = new Color(0.32f, 0.13f, 0.13f);
                        }
                        else
                        {
                            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                            player.nameText().color = tempColour;
                        }
                    }
                }
            }

            role.BlindButton.graphic.sprite = BlindSprite;
            role.BlindButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Blinded)
            {
                role.BlindButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.BlindDuration);
                role.BlindButton.graphic.color = Palette.EnabledColor;
                role.BlindButton.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            role.BlindButton.SetCoolDown(role.BlindTimer(), CustomGameOptions.BlindCd);

            if (!PlayerControl.LocalPlayer.moveable || role.BlindTimer() > 0f)
            {
                role.BlindButton.graphic.color = Palette.DisabledClear;
                role.BlindButton.graphic.material.SetFloat("_Desat", 1f);
                return;
            }

            role.BlindButton.graphic.color = Palette.EnabledColor;
            role.BlindButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}