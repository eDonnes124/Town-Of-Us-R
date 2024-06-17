using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MayorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class BodyguardUnbodyguard
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Mayor))
            {
                var mayor = (Mayor)role;
                if (mayor.Bodyguarded)
                    mayor.Bodyguard();
                else if (mayor.Enabled) mayor.UnBodyguard();
            }
        }
    }
}