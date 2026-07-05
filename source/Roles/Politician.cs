using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Politician : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> CampaignedPlayers = new List<byte>();
        public DateTime LastCampaigned;
        public bool CanCampaign;

        public Politician(PlayerControl player) : base(player)
        {
            Name = "Politician";
            ImpostorText = () => "Campaign To Become The Mayor!";
            TaskText = () => "Spread your campaign to become the Mayor!";
            Color = Patches.Colors.Politician;
            RoleType = RoleEnum.Politician;
            AddToRoleHistory(RoleType);
            CanCampaign = true;
            LastCampaigned = DateTime.UtcNow;
        }
        public GameObject RevealButton = new GameObject();

        public float CampaignTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCampaigned;
            var num = CustomGameOptions.CampaignCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0) return false;

            return true;
        }
    }
}