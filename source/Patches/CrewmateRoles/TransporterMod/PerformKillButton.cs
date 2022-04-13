using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TransporterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Transporter)) return true;
            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled) return false;
            if (role.TransportTimer() != 0f) return false;

            if (role.TransportList == null && role.ButtonUsable)
            {
                role.PressedButton = true;
                role.MenuClick = true;
            }

            return false;
        }
    }
}