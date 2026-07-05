using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class SoulCollector : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastReaped { get; set; }
        public List<GameObject> Souls = new List<GameObject>();
        public bool SCWins { get; set; }
        public SoulCollector(PlayerControl player) : base(player)
        {
            Name = "Soul Collector";
            ImpostorText = () => "Reap Souls";
            TaskText = () => "Reap souls";
            Color = Patches.Colors.SoulCollector;
            LastReaped = DateTime.UtcNow;
            RoleType = RoleEnum.SoulCollector;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public float ReapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastReaped;
            var num = CustomGameOptions.ReapCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var scTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            scTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = scTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.IsLover()) == 2) return false;
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.IsCrewKiller())) > 1) return false;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Crewmates))) == 1 ||
                PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2)
            {
                Utils.Rpc(CustomRPC.SoulCollectorWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            SCWins = true;
        }
    }
}