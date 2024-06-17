using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Politician : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> CampaignedPlayers = new List<byte>();
        public DateTime LastCampaigned;

        public int CampaignedAlive => CampaignedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);
        public bool IsElected => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected) <= CampaignedAlive;

        public Politician(PlayerControl player) : base(player)
        {
            Name = "Politician";
            ImpostorText = () => "Campaign To Become The Mayor!";
            TaskText = () => "Spread your campaign to become the Mayor!";
            Color = Patches.Colors.Politician;
            RoleType = RoleEnum.Politician;
            AddToRoleHistory(RoleType);
            CampaignedPlayers.Add(player.PlayerId);
        }

        public float CampaignTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCampaigned;
            var num = CustomGameOptions.CampaignCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void RpcSpreadCampaign(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);
            SpreadCampaign(source, target);
            Utils.Rpc(CustomRPC.Campaign, Player.PlayerId, source.PlayerId, target.PlayerId);
        }

        public void SpreadCampaign(PlayerControl source, PlayerControl target)
        {
            if (CampaignedPlayers.Contains(source.PlayerId) && !CampaignedPlayers.Contains(target.PlayerId)) CampaignedPlayers.Add(target.PlayerId);
            else if (CampaignedPlayers.Contains(target.PlayerId) && !CampaignedPlayers.Contains(source.PlayerId)) CampaignedPlayers.Add(source.PlayerId);
        }

        public void TurnMayor()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Mayor(Player);
            role.UsesLeft = 0;
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            if (Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Mayor));
                role.RegenTask();
            }
        }
    }
}