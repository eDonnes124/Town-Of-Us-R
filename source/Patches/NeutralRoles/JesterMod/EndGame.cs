using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(GameManager __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask) return true;

            foreach (var role in Role.AllRoles)
                if (role.RoleType == RoleEnum.Jester)
                    ((Jester)role).Loses();

            Utils.CallRpc(CustomRPC.JesterLose);

            return true;
        }
    }
}