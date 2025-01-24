using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<(byte, bool)> ListOfActives = new List<(byte, bool)>();


        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            ImpostorText = () => "Swap The Votes Of Two People";
            TaskText = () => "Swap two people's votes to save the Crew!";
            Color = Patches.Colors.Swapper;
            RoleType = RoleEnum.Swapper;
            AddToRoleHistory(RoleType);
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0) return false;

            return true;
        }
    }
}