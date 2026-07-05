using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving;
        public List<ArrowBehaviour> Arrows = new List<ArrowBehaviour>();
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            ImpostorText = () => "Bring The Crewmates Back From The Dead";
            TaskText = () => "Revive the Crewmates";
            Color = Patches.Colors.Altruist;
            RoleType = RoleEnum.Altruist;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.ReviveUses;
        }

        public bool UsedThisRound = false;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public float TimeRemaining;

        public bool ButtonUsable => UsesLeft != 0 && !UsedThisRound;

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0 &&
                ((UsesLeft > 0 && !UsedThisRound) || CurrentlyReviving) && GameObject.FindObjectsOfType<DeadBody>().Count > 0) return false;

            return true;
        }
    }
}