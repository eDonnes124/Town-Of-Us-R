using System;

namespace TownOfUs.Roles
{
    public class Warden : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Fortified;
        public DateTime StartingCooldown { get; set; }

        public Warden(PlayerControl player) : base(player)
        {
            Name = "Warden";
            ImpostorText = () => "Fortify Crewmates";
            TaskText = () => "Fortify the Crewmates";
            Color = Patches.Colors.Warden;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Warden;
            AddToRoleHistory(RoleType);
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
    }
}