using HarmonyLib;
using TownOfUs.Modifiers.AssassinMod;

namespace TownOfUs.CrewmateRoles.HunterMod
{
    public static class Retribution
    {
        public static PlayerControl LastVoted = null;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    internal class CastVote
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var votingPlayer = Utils.PlayerById(srcPlayerId);
            var suspectPlayer = Utils.PlayerById(suspectPlayerId);
            if (!suspectPlayer.Is(RoleEnum.Hunter)) return;
            Retribution.LastVoted = votingPlayer;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;
            if (exiled == null) return;
            var player = exiled.Object;

            if (player.Is(RoleEnum.Hunter))
            {
                if (Retribution.LastVoted != null && Retribution.LastVoted.PlayerId != player.PlayerId)
                {
                    Utils.Rpc(CustomRPC.Retribution, Retribution.LastVoted.PlayerId);
                    AssassinKill.MurderPlayer(Retribution.LastVoted);
                }
            }
            Retribution.LastVoted = null;
        }
    }
}