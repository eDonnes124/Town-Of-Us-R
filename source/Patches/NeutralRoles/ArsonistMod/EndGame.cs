using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix([HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask) return true;

            foreach (var role in Role.AllRoles)
                if (role.RoleType == RoleEnum.Arsonist)
                    ((Arsonist)role).Loses();

            Utils.CallRpc(CustomRPC.ArsonistLose);

            return true;
        }
    }
}