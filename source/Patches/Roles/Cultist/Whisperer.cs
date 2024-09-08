using System;
using System.Collections.Generic;

namespace TownOfUs.Roles.Cultist
{
    public class Whisperer : Role
    {

        public KillButton _whisperButton;
        public DateTime LastWhispered;
        public int WhisperCount;
        public int ConversionCount;
        public List<ConversionData> PlayerConversion = new();
        public int WhisperConversion;


        public Whisperer(PlayerControl player) : base(player)
        {
            Name = "Whisperer";
            ImpostorText = () => "Psst";
            TaskText = () => "Persuade Crewmates of your ideas";
            Color = Patches.Colors.Impostor;
            LastWhispered = DateTime.UtcNow;
            RoleType = RoleEnum.Whisperer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            WhisperConversion = CustomGameOptions.ConversionPercentage;
        }

        public KillButton WhisperButton
        {
            get => _whisperButton;
            set
            {
                _whisperButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float WhisperTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWhispered;
            var num = (CustomGameOptions.WhisperCooldown + CustomGameOptions.IncreasedCooldownPerWhisper * WhisperCount) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public List<ConversionData> GetPlayers()
        {
            var playerList = new List<ConversionData>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.CultistSeer) |
                    player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Whisperer)))
                {
                    playerList.Add(new(player, 100));
                }
            }
            return playerList;
        }
    }
}