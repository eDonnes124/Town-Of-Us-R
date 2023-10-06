using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.ImpostorRoles.GrenadierMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);
            if (__instance == role.FlashButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);
                if (sabActive) return false;
                if (role.FlashTimer() != 0) return false;

                Utils.Rpc(CustomRPC.FlashGrenade, PlayerControl.LocalPlayer.PlayerId);
                role.TimeRemaining = CustomGameOptions.GrenadeDuration;
                role.Flash();
                return false;
            }

            return true;
        }
    }
}