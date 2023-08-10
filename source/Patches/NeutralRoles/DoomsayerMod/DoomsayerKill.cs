using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.DoomsayerMod
{
    public class DoomsayerKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl doomsayer)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, doomsayer);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl doomsayer)
        {
            DoomKillCount(doomsayer);
            (Role.GetRole<Doomsayer>(doomsayer) as IGuesser).MurderPlayer(voteArea, player);
            Utils.Rpc(CustomRPC.DoomsayerKill, player.PlayerId, doomsayer.PlayerId);
        }

        public static void DoomKillCount(PlayerControl doomsayer)
        {
            var doom = Role.GetRole<Doomsayer>(doomsayer);
            doom.CorrectAssassinKills += 1;
            doom.GuessedCorrectly += 1;
            if (doom.GuessedCorrectly == CustomGameOptions.DoomsayerGuessesToWin) doom.WonByGuessing = true;
        }
    }
}
