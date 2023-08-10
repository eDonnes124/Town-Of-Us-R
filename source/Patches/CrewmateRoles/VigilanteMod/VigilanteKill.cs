using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VigilanteMod
{
    public class VigilanteKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl vigilante)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, vigilante);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl vigilante)
        {
            (Role.GetRole<Vigilante>(vigilante) as IGuesser).MurderPlayer(voteArea, player);
            VigiKillCount(player, vigilante);
            Utils.Rpc(CustomRPC.VigilanteKill, player.PlayerId, vigilante.PlayerId);
        }

        public static void VigiKillCount(PlayerControl player, PlayerControl vigilante)
        {
            var vigi = Role.GetRole<Vigilante>(vigilante);
            if (player == vigilante) vigi.IncorrectAssassinKills += 1;
            else vigi.CorrectAssassinKills += 1;
        }
    }
}
