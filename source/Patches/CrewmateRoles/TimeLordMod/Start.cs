using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var TimeLord = (TimeLord) role;
                TimeLord.FinishRewind = DateTime.UtcNow;
                TimeLord.StartRewind = DateTime.UtcNow;
                TimeLord.FinishRewind = TimeLord.FinishRewind.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown);
                TimeLord.StartRewind = TimeLord.StartRewind.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown);
                TimeLord.StartRewind = TimeLord.StartRewind.AddSeconds(-10.0f);
            }
        }
    }
}