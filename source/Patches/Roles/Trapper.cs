using System;
using System.Collections.Generic;
using TMPro;
using TownOfUs.CrewmateRoles.TrapperMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Trapper : Role
    {
        public static Material trapMaterial = TownOfUs.bundledAssets.Get<Material>("trap");

        public List<Trap> traps = new List<Trap>();
        public DateTime LastTrapped { get; set; }
        public int UsesLeft;
        public TextMeshPro UsesText;

        public List<RoleEnum> trappedPlayers;

        public bool ButtonUsable => UsesLeft != 0;
        public Trapper(PlayerControl player) : base(player)
        {
            Name = "Trapper";
            ImpostorText = () => "Catch Killers In The Act";
            TaskText = () => "Place traps around the map";
            Color = Patches.Colors.Trapper;
            RoleType = RoleEnum.Trapper;
            LastTrapped = DateTime.UtcNow;
            trappedPlayers = new List<RoleEnum>();
            AddToRoleHistory(RoleType);

            UsesLeft = CustomGameOptions.MaxTraps;
        }

        public float TrapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTrapped;
            var num = CustomGameOptions.TrapCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
