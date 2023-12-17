using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VeteranMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Alert
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Veteran);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var alertButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == alertButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.AlertTimer() != 0) return false;
                role.TimeRemaining = CustomGameOptions.AlertDuration;
                role.UsesLeft--;
                role.Alert();
                Utils.Rpc(CustomRPC.Alert, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}