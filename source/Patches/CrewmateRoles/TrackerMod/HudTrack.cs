using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HudTrack
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateTrackButton(__instance);
        }

        public static void UpdateTrackButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var trackButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(trackButton.cooldownTimerText, trackButton.transform);
                role.UsesText.gameObject.SetActive(true);
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
            if (isDead)
            {
                trackButton.gameObject.SetActive(false);
                // trackButton.isActive = false;
            }
            else
            {
                trackButton.gameObject.SetActive(!MeetingHud.Instance);
                // trackButton.isActive = !MeetingHud.Instance;
                trackButton.SetCoolDown(role.TrackerTimer(), CustomGameOptions.TrackCd);
                if (role.UsesLeft == 0) return;

                var notTracked = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(x => !role.IsTracking(x))
                    .ToList();

                Utils.SetTarget(ref role.ClosestPlayer, trackButton, float.NaN, notTracked);
            }

            var renderer = trackButton.graphic;
            if (role.ClosestPlayer != null && role.ButtonUsable)
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
        }
    }
}