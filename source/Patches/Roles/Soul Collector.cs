using System;
using System.Collections.Generic;
using TMPro;
using TownOfUs.NeutralRoles.SoulCollectorMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class SoulCollector : Role
    {
        private KillButton _reapButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastReaped { get; set; }
        public Soul CurrentTarget;
        public List<GameObject> Souls = new List<GameObject>();
        public bool CollectedSouls = false;
        public int SoulsCollected = 0;
        public List<byte> ReapedPlayers = new List<byte>();
        public TextMeshPro CollectedText { get; set; }

        public SoulCollector(PlayerControl player) : base(player)
        {
            Name = "Soul Collector";
            ImpostorText = () => "Collect Souls";
            TaskText = () => "Collect souls to win the game";
            Color = Patches.Colors.SoulCollector;
            LastReaped = DateTime.UtcNow;
            RoleType = RoleEnum.SoulCollector;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        public KillButton ReapButton
        {
            get => _reapButton;
            set
            {
                _reapButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.SoulCollectorEndsGame) return true;
            if (!CollectedSouls) return true;
            Utils.EndGame();
            return false;
        }
    }
}