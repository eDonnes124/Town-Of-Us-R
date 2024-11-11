using HarmonyLib;
using Reactor.Utilities;
using System.Linq;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;

            var role = Role.GetRole(player);
            if (role == null) return;
            if (role.RoleType == RoleEnum.Jester)
            {
                var jesterRole = (Jester)role;
                jesterRole.Wins();
                

                if (CustomGameOptions.JesterEndsGame || !CustomGameOptions.JesterHaunt) return;
                if (PlayerControl.LocalPlayer != player) return;

                jesterRole.KillableVoters = MeetingHud.Instance.playerStates.Where(x => !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.Pestilence) && x.VotedFor == player.PlayerId).Select(x => x.TargetPlayerId).ToArray();

                if (CustomGameOptions.JesterChoosesHaunt) return;

                jesterRole.PauseEndCrit = true;
                var pk = new PlayerMenu((x) =>
                {
                    Utils.RpcMultiMurderPlayer(player, x);
                    role.PauseEndCrit = false;
                }, (y) =>
                {
                    return jesterRole.KillableVoters.Contains(y.PlayerId);
                });

                Coroutines.Start(pk.Open(3f));
            }
        }
    }
}