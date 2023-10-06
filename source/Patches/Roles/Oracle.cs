using System;

namespace TownOfUs.Roles
{
    public class Oracle : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Confessor;
        public float Accuracy;
        public bool FirstMeetingDead;
        public Faction RevealedFaction;
        public bool SavedConfessor;
        public DateTime LastConfessed { get; set; }

        public Oracle(PlayerControl player) : base(player)
        {
            Name = "Oracle";
            ImpostorText = () => "Get Other Player's To Confess Their Sins";
            TaskText = () => "Get another player to confess on your passing";
            Color = Patches.Colors.Oracle;
            LastConfessed = DateTime.UtcNow;
            Accuracy = CustomGameOptions.RevealAccuracy;
            FirstMeetingDead = true;
            FirstMeetingDead = false;
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
    }
}