using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(GameManager __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask)
            {
                foreach (var role in Role.AllRoles)
                    if (role.RoleType == RoleEnum.Survivor && !role.Player.Data.IsDead && !role.Player.Data.Disconnected)
                    {
                        ((Survivor)role).AliveImpWin();

                        Utils.CallRpc(CustomRPC.SurvivorImpWin);
                        return true;
                    }
            }
            else
            {
                foreach (var role in Role.AllRoles)
                    if (role.RoleType == RoleEnum.Survivor && (role.Player.Data.IsDead || role.Player.Data.Disconnected))
                    {
                        ((Survivor)role).DeadCrewWin();

                        Utils.CallRpc(CustomRPC.SurvivorCrewWin);
                        return true;
                    }
            }
            return true;
        }
    }
}