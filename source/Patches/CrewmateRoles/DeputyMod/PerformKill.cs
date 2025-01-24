using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Deputy);
            if (!flag) return true;
            var role = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled) return false;
            if (role.ClosestPlayer == null || role.Camping != null) return false;
            if (role.StartTimer() > 0 || role.CampedThisRound) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Camping = role.ClosestPlayer;
                role.CampedThisRound = true;
                Utils.Rpc(CustomRPC.Camp, PlayerControl.LocalPlayer.PlayerId, (byte)0, role.Camping.PlayerId);
                return false;
            }
            return false;
        }
    }
}
