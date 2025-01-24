using System;
using TMPro;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Lookout : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastWatched { get; set; }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
        public Dictionary<byte, List<RoleEnum>> Watching { get; set; } = new();

        public Lookout(PlayerControl player) : base(player)
        {
            Name = "Lookout";
            ImpostorText = () => "Keep Your Eyes Wide Open";
            TaskText = () => "Watch other crewmates";
            Color = Patches.Colors.Lookout;
            LastWatched = DateTime.UtcNow;
            RoleType = RoleEnum.Lookout;
            AddToRoleHistory(RoleType);

            UsesLeft = CustomGameOptions.MaxWatches;
        }

        public float WatchTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWatched;
            var num = CustomGameOptions.WatchCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}