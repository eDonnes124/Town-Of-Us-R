using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MercenaryMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HUDMercShield
    {
        public static Sprite ArmorSprite => TownOfUs.DonArmorSprite;

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateButtons(__instance);
        }

        public static void UpdateButtons(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary)) return;

            var role = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
            var protectButton = __instance.KillButton;
            if (role.ArmorButton == null)
            {
                role.ArmorButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ArmorButton.graphic.enabled = true;
                role.ArmorButton.graphic.sprite = ArmorSprite;
                role.ArmorButton.gameObject.SetActive(false);
            }
            role.ArmorButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            role.ArmorButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.BrildersText == null)
            {
                role.BrildersText = Object.Instantiate(role.ArmorButton.cooldownTimerText, role.ArmorButton.transform);
                role.BrildersText.gameObject.SetActive(false);
                role.BrildersText.transform.localPosition = new Vector3(
                    role.BrildersText.transform.localPosition.x + 0.26f,
                    role.BrildersText.transform.localPosition.y + 0.29f,
                    role.BrildersText.transform.localPosition.z);
                role.BrildersText.transform.localScale = role.BrildersText.transform.localScale * 0.65f;
                role.BrildersText.alignment = TMPro.TextAlignmentOptions.Right;
                role.BrildersText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.BrildersText != null)
            {
                role.BrildersText.text = role.Brilders.ToString();
            }
            role.BrildersText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.Armored) role.ArmorButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ArmorDuration);
            else if (role.Brilders > 0) role.ArmorButton.SetCoolDown(role.ArmorTimer(), CustomGameOptions.ArmorCd);
            else role.ArmorButton.SetCoolDown(0f, CustomGameOptions.ArmorCd);

            var renderer = role.ArmorButton.graphic;
            if (role.Armored || (!role.ArmorButton.isCoolingDown && role.Brilders > 0))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.BrildersText.color = Palette.EnabledColor;
                role.BrildersText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.BrildersText.color = Palette.DisabledClear;
                role.BrildersText.material.SetFloat("_Desat", 1f);
            }

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            protectButton.SetCoolDown(role.StartTimer(), CustomGameOptions.ProtectAbsorbCd);
            if (role.Armored) return;
            var notShielded = PlayerControl.AllPlayerControls.ToArray().Where(
                player => role.ShieldedPlayer != player
            ).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, protectButton, float.NaN, notShielded);
        }
    }
}
