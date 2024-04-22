using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.HunterMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        public static Sprite StalkSprite => TownOfUs.StalkSprite;
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateKillButtons(__instance);
        }

        public static void UpdateKillButtons(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) return;

            var role = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);

            foreach (var player in role.CaughtPlayers)
            {
                var data = player.Data;
                if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                    continue;
                player.nameText().color = Color.black;
            }

            if (role.StalkButton == null)
            {
                role.StalkButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StalkButton.graphic.enabled = true;
                role.StalkButton.gameObject.SetActive(false);
            }

            role.StalkButton.graphic.sprite = StalkSprite;
            role.StalkButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(role.StalkButton.cooldownTimerText, role.StalkButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }

            role.StalkButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Stalking) role.StalkButton.SetCoolDown(role.StalkDuration, CustomGameOptions.HunterStalkDuration);
            else if (role.StalkUsable) role.StalkButton.SetCoolDown(role.StalkTimer(), CustomGameOptions.HunterStalkCd);
            else role.StalkButton.SetCoolDown(0f, CustomGameOptions.HunterStalkCd);
            var notCaught = PlayerControl.AllPlayerControls.ToArray().Where(
                pc => !role.CaughtPlayers.Contains(pc)
            ).ToList();
            Utils.SetTarget(ref role.ClosestStalkPlayer, role.StalkButton, float.NaN, notCaught);

            var renderer = role.StalkButton.graphic;
            if (role.Stalking || (!role.StalkButton.isCoolingDown && role.StalkUsable && role.ClosestStalkPlayer != null))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }

            var killButton = __instance.KillButton;

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.HunterKillTimer(), CustomGameOptions.HunterKillCd);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, role.CaughtPlayers);
        }
    }
}