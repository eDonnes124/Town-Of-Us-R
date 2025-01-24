using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.WardenMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Warden);
            if (!flag) return true;
            var role = Role.GetRole<Warden>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled) return false;
            if (role.ClosestPlayer == null || role.Fortified != null) return false;
            if (role.StartTimer() > 0) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Fortified = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Fortify, (byte)0, PlayerControl.LocalPlayer.PlayerId, role.Fortified.PlayerId);
                return false;
            }
            return false;
        }
    }
}
