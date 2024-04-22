using System;
using UnityEngine;
using TMPro;

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
            var survTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            survTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = survTeam;
        }

    }
}