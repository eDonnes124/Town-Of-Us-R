using System;

namespace TownOfUs.Roles.Cultist
{
    public class Necromancer : Role
    {
        public DeadBody CurrentTarget;
        public DateTime LastRevived;
        public int ReviveCount;


        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            ImpostorText = () => "Revive The Dead To Do Your Dirty Work";
            TaskText = () => "Revive Crewmates to turn them into Impostors";
            Color = Patches.Colors.Impostor;
            LastRevived = DateTime.UtcNow;
            RoleType = RoleEnum.Necromancer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public float ReviveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRevived;
            var num = (CustomGameOptions.ReviveCooldown + CustomGameOptions.IncreasedCooldownPerRevive * ReviveCount) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}