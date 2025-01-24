using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Imitator : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<(byte, bool)> ListOfActives = new List<(byte, bool)>();
        public PlayerControl ImitatePlayer = null;

        public List<RoleEnum> trappedPlayers = null;
        public Dictionary<byte, List<RoleEnum>> watchedPlayers = null;
        public PlayerControl confessingPlayer = null;
        public PlayerControl jailedPlayer = null;


        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            ImpostorText = () => "Use The True-Hearted Dead To Benefit The Crew";
            TaskText = () => "Use dead roles to benefit the crew";
            Color = Patches.Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            AddToRoleHistory(RoleType);
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0 &&
                PlayerControl.AllPlayerControls.ToArray().Count(x => x.Data.IsDead && !x.Data.Disconnected &&
                (x.Is(RoleEnum.Hunter) || x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Veteran))) > 0) return false;

            return true;
        }
    }
}