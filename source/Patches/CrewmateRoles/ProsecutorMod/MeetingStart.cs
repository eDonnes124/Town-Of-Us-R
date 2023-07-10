using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ProsecutorMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            foreach (var pros in Role.GetRoles(RoleEnum.Prosecutor))
            {
                var prosRole = (Prosecutor)pros;
                prosRole.StartProsecute = false;
            }
            return;
        }
    }
}
