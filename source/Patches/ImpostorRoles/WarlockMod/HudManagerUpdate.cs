using HarmonyLib;
using System;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.WarlockMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warlock)) return;
            var role = Role.GetRole<Warlock>(PlayerControl.LocalPlayer);

            if (role.ChargeText == null)
            {
                role.ChargeText = UnityEngine.Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.ChargeText.gameObject.SetActive(false);
                role.ChargeText.transform.localPosition = new Vector3(
                    role.ChargeText.transform.localPosition.x + 0.26f,
                    role.ChargeText.transform.localPosition.y + 0.29f,
                    role.ChargeText.transform.localPosition.z);
                role.ChargeText.transform.localScale = role.ChargeText.transform.localScale * 0.65f;
                role.ChargeText.alignment = TMPro.TextAlignmentOptions.Right;
                role.ChargeText.fontStyle = TMPro.FontStyles.Bold;
                role.ChargeText.enableWordWrapping = false;
            }
            if (role.ChargeText != null)
            {
                role.ChargeText.text = role.ChargePercent + "%";
            }
            role.ChargeText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && (role.Charging || role.UsingCharge));
            if (role.UsingCharge)
            {
                if (role.Charging)
                {
                    role.StartUseTime = DateTime.UtcNow;
                    role.Charging = false;
                }
            }
            else if (PlayerControl.LocalPlayer.killTimer == 0f)
            {
                if (!role.Charging)
                {
                    role.StartChargeTime = DateTime.UtcNow;
                    role.Charging = true;
                }
            }
            else
            {
                role.ChargePercent = 0;
                role.Charging = false;
                role.UsingCharge = false;
            }
            return;
        }
    }
}