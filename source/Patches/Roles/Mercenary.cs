using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace TownOfUs.Roles
{
    public class Mercenary : Role
    {
        public KillButton _armorButton;
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public DateTime StartingCooldown { get; set; }
        public int Brilders { get; set; }
        public TextMeshPro BrildersText;
        public bool Enabled { get; set; }
        public DateTime LastArmored { get; set; }
        public float TimeRemaining { get; set; }
        public Mercenary(PlayerControl player) : base(player)
        {
            Name = "Mercenary";
            ImpostorText = () => "Collect Brilders And Survive";
            TaskText = () => (HasEnoughBrilders
                ? "Stay alive to win!"
                : "Stop abilities with your shield to gain brilders")
                + "\nFake Tasks:";
            Color = Patches.Colors.Mercenary;
            StartingCooldown = DateTime.UtcNow;
            LastArmored = DateTime.UtcNow;
            RoleType = RoleEnum.Mercenary;
            Faction = Faction.NeutralBenign;
            AddToRoleHistory(RoleType);
            ShieldedPlayer = null;
        }
        public bool HasEnoughBrilders
        {
            get
            {
                return Brilders >= CustomGameOptions.MercenaryBrildersRequired;
            }
        }

        public KillButton ArmorButton
        {
            get => _armorButton;
            set
            {
                _armorButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
        public bool Armored => TimeRemaining > 0f;

        public float ArmorTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastArmored;
            var num = CustomGameOptions.ArmorCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void DonArmor()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void RemoveArmor()
        {
            Enabled = false;
            LastArmored = DateTime.UtcNow;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var mercTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            mercTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = mercTeam;
        }

        public PlayerControl ClosestPlayer;
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }
    }
}