using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.SwooperMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Swooper);
            if (!flag) return true;
            if (RoleManager.IsGhostRole(PlayerControl.LocalPlayer.Data.RoleType)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
            if (__instance.isCoolingDown) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.SwoopTimer() != 0) return false;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Swoop, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            role.TimeRemaining = CustomGameOptions.SwoopDuration;
            role.Swoop();
            return false;
        }
    }
}