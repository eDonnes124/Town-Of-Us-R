using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
namespace TownOfUs.Modifiers.AssassinMod
{
    public class AssassinKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl assassin)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, assassin);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl assassin)
        {
            (Ability.GetAbility<Assassin>(assassin) as IGuesser).MurderPlayer(voteArea, player);
            AssassinKillCount(player, assassin);
            Utils.Rpc(CustomRPC.AssassinKill, player.PlayerId, assassin.PlayerId);
        }

        public static void AssassinKillCount(PlayerControl player, PlayerControl assassin)
        {
            var assassinPlayer = Role.GetRole(assassin);
            if (player == assassin) assassinPlayer.IncorrectAssassinKills += 1;
            else assassinPlayer.CorrectAssassinKills += 1;
        }
    }
}
