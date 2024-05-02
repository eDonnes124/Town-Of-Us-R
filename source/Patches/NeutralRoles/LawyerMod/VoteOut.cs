using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Modifiers.AssassinMod;
using UnityEngine;
using Reactor.Utilities.Extensions;

namespace TownOfUs.NeutralRoles.LawyerMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;
            if (exiled == null) return;
            var player = exiled.Object;

            foreach (var role in Role.GetRoles(RoleEnum.Lawyer))
            {
                var lwyrRole = ((Lawyer)role);
                if (player.PlayerId == lwyrRole.target.PlayerId && CustomGameOptions.LawyerDies)
                {
                    lwyrRole.TargetVotedOut = true;
                    if (!lwyrRole.Player.Data.IsDead)
                    {
                        AssassinKill.MurderPlayer(role.Player, role.Player);
                        role.IncorrectAssassinKills -= 1;
                    }
                }
            }
        }
    }
}