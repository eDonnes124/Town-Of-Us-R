using System.Collections.Generic;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using System;
using Reactor.Utilities;

namespace TownOfUs.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();
        public readonly List<byte> PreviouslyInvestigated = new List<byte>();
        public PlayerControl InvestigatedPlayer;
        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }
        public bool StartRemoving { get; set; } = false;
        public int RoleCount { get; set; } = CustomGameOptions.RoleAmount;
        public int NarrowCount { get; set; } = CustomGameOptions.RemoveRoles;
        public bool ShouldRemove { get; set; } = false;
        public bool ShouldNotify { get; set; } = false;


        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => "Find All Impostors By Examining Footprints";
            TaskText = () => "You can see everyone's footprints";
            Color = Patches.Colors.Investigator;
            RoleType = RoleEnum.Investigator;
            AddToRoleHistory(RoleType);
            Scale = 1.4f;
            InvestigatedPlayer = null;
        }

        public float InvestigateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.InvestCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}