using System;

namespace TownOfUs.Roles
{
    public class Oracle : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Confessor;
        public float Accuracy;
        public Faction RevealedFaction;
        public bool SavedBlessed;
        public KillButton _blessButton;

        public PlayerControl ClosestBlessedPlayer;
        public PlayerControl Blessed;
        public DateTime LastConfessed { get; set; }
        public DateTime LastBlessed { get; set; }

        public Oracle(PlayerControl player) : base(player)
        {
            Name = "Oracle";
            ImpostorText = () => "Get Other Players To Confess Their Sins";
            TaskText = () => "Get another player to confess on your passing";
            Color = Patches.Colors.Oracle;
            LastConfessed = DateTime.UtcNow;
            LastBlessed = DateTime.UtcNow;
            Accuracy = CustomGameOptions.RevealAccuracy;
            RoleType = RoleEnum.Oracle;
            AddToRoleHistory(RoleType);
        }
        public float ConfessTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConfessed;
            var num = CustomGameOptions.ConfessCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton BlessButton
        {
            get => _blessButton;
            set
            {
                _blessButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float BlessTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlessed;
            var num = CustomGameOptions.BlessCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}