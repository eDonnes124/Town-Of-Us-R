using System;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Miner : Role, IExtraButton
    {
        public readonly List<Vent> Vents = new();

        public KillButton RoleAbilityButton { get; set; }
        public DateTime LastMined;


        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            ImpostorText = () => "From The Top, Make It Drop, That's A Vent";
            TaskText = () => "Place vents around the map";
            Color = Patches.Colors.Impostor;
            LastMined = DateTime.UtcNow;
            RoleType = RoleEnum.Miner;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = CustomGameOptions.MineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}