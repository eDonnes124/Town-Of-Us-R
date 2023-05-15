using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix([HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask) return true;

            foreach (var role in Role.AllRoles)
                if (role.RoleType == RoleEnum.Amnesiac)
                    ((Amnesiac)role).Loses();
            Utils.CallRpc(CustomRPC.AmnesiacLose);

            return true;
        }
    }
}