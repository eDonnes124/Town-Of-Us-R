using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    public class EndGame
    {
        public static void Reset()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Investigator)) ((Investigator) role).AllPrints.Clear();
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        
        public static class EndGamePatch
        {
            public static void Prefix()
            {
                Reset();
            }
        }
    }
}