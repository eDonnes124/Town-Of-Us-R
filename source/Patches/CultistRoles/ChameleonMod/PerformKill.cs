using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;

namespace TownOfUs.CultistRoles.ChameleonMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Alert
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);
            var swoopButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == swoopButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.SwoopTimer() != 0) return false;
                role.TimeRemaining = CustomGameOptions.SwoopDuration;
                role.Swoop();
                Utils.Rpc(CustomRPC.ChameleonSwoop, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}