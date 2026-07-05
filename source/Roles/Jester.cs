using Il2CppSystem.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        public bool SpawnedAs = true;

        public DateTime LastMoved;
        public TextMeshPro TimerText;
        public List<Vector3> Locations = new List<Vector3>();

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => "Get Voted Out";
            TaskText = () => SpawnedAs ? "Get voted out!\nFake Tasks:" : "Your target was killed. Now you get voted out!\nFake Tasks:";
            Color = Patches.Colors.Jester;
            RoleType = RoleEnum.Jester;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
            LastMoved = DateTime.UtcNow;
            Locations.Add(Player.transform.localPosition);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            if (CustomGameOptions.JesterWin != NeutralRoles.ExecutionerMod.WinEndsGame.EndsGame) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }

        public float ScatterTimer()
        {
            if (MeetingHud.Instance)
            {
                LastMoved = DateTime.UtcNow;
                return CustomGameOptions.JestScatterTimer;
            }
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMoved;
            var num = CustomGameOptions.JestScatterTimer * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}