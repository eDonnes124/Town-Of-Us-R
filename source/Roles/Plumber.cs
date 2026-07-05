using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Plumber : Role
    {
        public DateTime LastFlushed;
        public Vent Vent;
        public List<byte> FutureBlocks = new List<byte>();
        public List<byte> VentsBlocked = new List<byte>();
        public List<GameObject> Barricades = new List<GameObject>();

        public Plumber(PlayerControl player) : base(player)
        {
            Name = "Plumber";
            ImpostorText = () => "Get The Rats Out Of The Sewers";
            TaskText = () => "Maintain a clean vent system";
            Color = Patches.Colors.Plumber;
            RoleType = RoleEnum.Plumber;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.MaxBarricades;
            LastFlushed = DateTime.UtcNow;
        }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public float FlushTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlushed;
            var num = CustomGameOptions.FlushCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}