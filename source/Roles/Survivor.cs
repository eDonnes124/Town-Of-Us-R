using System;
using UnityEngine;
using TMPro;
using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Survivor : Role
    {
        public bool Enabled;
        public DateTime LastVested;
        public float TimeRemaining;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
        public bool SpawnedAs = true;

        public DateTime LastMoved;
        public TextMeshPro TimerText;
        public List<Vector3> Locations = new List<Vector3>();


        public Survivor(PlayerControl player) : base(player)
        {
            Name = "Survivor";
            ImpostorText = () => "Do Whatever It Takes To Live";
            TaskText = () => SpawnedAs ? "Stay alive to win" : "Your target was killed. Now you just need to live!";
            Color = Patches.Colors.Survivor;
            LastVested = DateTime.UtcNow;
            RoleType = RoleEnum.Survivor;
            Faction = Faction.NeutralBenign;
            AddToRoleHistory(RoleType);

            UsesLeft = CustomGameOptions.MaxVests;
            LastMoved = DateTime.UtcNow;
            Locations.Add(Player.transform.localPosition);
        }

        public bool Vesting => TimeRemaining > 0f;

        public float VestTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastVested;
            var num = CustomGameOptions.VestCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Vest()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnVest()
        {
            Enabled = false;
            LastVested = DateTime.UtcNow;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var survTeam = new List<PlayerControl>();
            survTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = survTeam;
        }

        public float ScatterTimer()
        {
            if (MeetingHud.Instance)
            {
                LastMoved = DateTime.UtcNow;
                return CustomGameOptions.SurvScatterTimer;
            }
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMoved;
            var num = CustomGameOptions.SurvScatterTimer * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}