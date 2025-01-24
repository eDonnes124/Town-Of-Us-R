using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Deputy : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Camping = null;
        public PlayerControl Killer = null;
        public bool CampedThisRound = false;
        public DateTime StartingCooldown { get; set; }
        public Dictionary<byte, GameObject> Buttons { get; set; } = new();

        public Deputy(PlayerControl player) : base(player)
        {
            Name = "Deputy";
            ImpostorText = () => "Camp Crewmates To Catch Their Killer";
            TaskText = () => "Camp crewmates then shoot their killer";
            Color = Patches.Colors.Deputy;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Deputy;
            AddToRoleHistory(RoleType);
        }

        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0
                && Killer != null && !Killer.Data.IsDead && !Killer.Data.Disconnected) return false;

            return true;
        }
    }
}