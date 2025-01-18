using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TMPro;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Tracker : Role
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public DateTime LastTracked;
        public DateTime LastStartedTracking;

        public int UsesLeft;
        public float TimeRemaining;
        public float TrackDuration;
        public bool isTrackingPlayer;
        public TextMeshPro UsesText;
        public KillButton _trackButton;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            ImpostorText = () => "Track Everyone's Movement";
            TaskText = () => "Track suspicious players";
            Color = Patches.Colors.Tracker;
            LastTracked = DateTime.UtcNow;
            RoleType = RoleEnum.Tracker;
            AddToRoleHistory(RoleType);

            TrackDuration = CustomGameOptions.TrackDuration;
            isTrackingPlayer = false;
        }

        public KillButton TrackButton
        {
            get => _trackButton;
            set
            {
                _trackButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float TrackerTimer() {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = CustomGameOptions.TrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TrackerDuration() {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStartedTracking;
            var num = CustomGameOptions.TrackDuration * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public void TrackPlayer(PlayerControl target) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;

            PlayerControl.LocalPlayer.StartPlayerTracking(target, target.GetDefaultOutfit().ColorId);
        }

        public void StopTrackingPlayer() {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;

            LastTracked = DateTime.UtcNow;
            isTrackingPlayer = false;
            PlayerControl.LocalPlayer.CancelPlayerTracking();
        }
    }
}