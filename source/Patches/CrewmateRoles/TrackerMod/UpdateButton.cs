using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateButton
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateTrackButton(__instance);
        }
        public static void UpdateTrackButton(PlayerControl __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var trackButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (isDead) {
                trackButton.gameObject.SetActive(false);
            }
            else {
                System.Console.WriteLine($"isTrackingPlayer? {role.isTrackingPlayer}");
                System.Console.WriteLine($"TrackerTimer? {role.TrackerTimer}");
                System.Console.WriteLine($"TrackerDuration? {role.TrackDuration}");
                System.Console.WriteLine($"LastTracked? {role.LastTracked}");
                System.Console.WriteLine($"LastStartedTracking? {role.LastStartedTracking}");
                trackButton.gameObject.SetActive(!MeetingHud.Instance);

                if (role.isTrackingPlayer && role.TrackerDuration() <= 0) {
                    role.LastTracked = DateTime.UtcNow;
                    //role.StopTrackingPlayer();
                }
                else if (role.isTrackingPlayer) {
                    role.TrackButton.SetCoolDown(role.TrackerDuration(), CustomGameOptions.TrackDuration);
                    return;
                }

                trackButton.SetCoolDown(role.TrackerTimer(), CustomGameOptions.TrackCd);

                Utils.SetTarget(ref role.ClosestPlayer, trackButton);
                
                //role.TrackButton.graphic.color = Palette.EnabledColor;
                //role.TrackButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
