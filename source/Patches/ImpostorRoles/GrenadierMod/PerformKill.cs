using HarmonyLib;
using Hazel;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.GrenadierMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(AbilityButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier);
            if (!flag) return true;
            if (RoleManager.IsGhostRole(PlayerControl.LocalPlayer.Data.RoleType)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);
            if (__instance.graphic.sprite == TownOfUs.FlashSprite)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);
                if (sabActive) return false;
                if (role.FlashTimer() != 0) return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.FlashGrenade, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.GrenadeDuration;
                role.Flash();
                return false;
            }

            return true;
        }
    }
}