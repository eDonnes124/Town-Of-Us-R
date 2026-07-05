using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Eclipsal : Role
    {
        public KillButton _blindButton;
        public DateTime LastBlind;
        public float visionPerc = 1f;
        public float TimeRemaining;
        public bool Enabled = false;

        public Il2CppSystem.Collections.Generic.List<PlayerControl> BlindPlayers = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        public Eclipsal(PlayerControl player) : base(player)
        {
            Name = "Eclipsal";
            ImpostorText = () => "Block Out The Light";
            TaskText = () => "Make Crewmates unable to see";
            Color = Patches.Colors.Impostor;
            LastBlind = DateTime.UtcNow;
            RoleType = RoleEnum.Eclipsal;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public bool Blinded => TimeRemaining > 0f;


        public KillButton BlindButton
        {
            get => _blindButton;
            set
            {
                _blindButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float BlindTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlind;
            var num = CustomGameOptions.BlindCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void GetBlinds()
        {
            BlindPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.BlindRadius, false);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Impostors)) BlindPlayers.Remove(player);
            }
            Blind();
        }

        public void Blind()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (TimeRemaining > CustomGameOptions.BlindDuration - 1f)
            {
                visionPerc = TimeRemaining - CustomGameOptions.BlindDuration + 1f;
            }
            else if (TimeRemaining < 1f)
            {
                visionPerc = 1f - TimeRemaining;
            }
            else visionPerc = 0f;
        }

        public void UnBlind()
        {
            Enabled = false;
            LastBlind = DateTime.UtcNow;
            BlindPlayers.Clear();
            visionPerc = 1f;
        }
    }
}