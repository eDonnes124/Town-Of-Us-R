using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MayorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AutoReveal
    {
        public static void Postfix(MeetingHud __instance)
        {
            foreach(var _role in Role.GetRoles(RoleEnum.Mayor))
            {
                Mayor role = (Mayor)_role;
                if (!role.Revealed)
                {
                    role.Revealed = true;
                    AddRevealButton.RemoveAssassin(role);
                }
            }
        }
    }
}