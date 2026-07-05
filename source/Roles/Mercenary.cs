using System;
using System.Collections.Generic;
using TMPro;

namespace TownOfUs.Roles
{
    public class Mercenary : Role
    {
        public bool Enabled;
        public DateTime LastGuarded;

        public TextMeshPro UsesText;
        public TextMeshPro GoldText;

        public bool SpawnedAs = true;
        public PlayerControl ClosestGuardPlayer;
        public PlayerControl ClosestBribePlayer;

        public List<byte> Guarded = new List<byte>();
        public List<byte> Bribed = new List<byte>();
        private KillButton _guardButton;
        public int UsesLeft => CustomGameOptions.MaxGuards - Guarded.Count;
        public bool ButtonUsable => UsesLeft != 0;
        public int Gold = 0;

        public bool Alert = false;

        public Mercenary(PlayerControl player) : base(player)
        {
            Name = "Mercenary";
            ImpostorText = () => "Bribe The Crewmates";
            TaskText = () => SpawnedAs ? "Bribe the Crewmates to win" : "Your target was killed. Now bribe the Crewmates to win!";
            Color = Patches.Colors.Mercenary;
            LastGuarded = DateTime.UtcNow;
            RoleType = RoleEnum.Mercenary;
            Faction = Faction.NeutralBenign;
            AddToRoleHistory(RoleType);
        }

        public KillButton GuardButton
        {
            get => _guardButton;
            set
            {
                _guardButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float GuardTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastGuarded;
            var num = CustomGameOptions.GuardCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var mercTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            mercTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = mercTeam;
        }

    }
}