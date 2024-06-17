using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MayorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Bodyguard
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var bodyguardButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == bodyguardButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.BodyguardTimer() != 0) return false;
                role.TimeRemaining = CustomGameOptions.BodyguardDuration;
                role.UsesLeft--;
                role.Bodyguard();
                Utils.Rpc(CustomRPC.Bodyguard, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}