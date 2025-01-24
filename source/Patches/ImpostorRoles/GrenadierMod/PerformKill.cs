using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUs.Roles;

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
                if (role.Player.inVent) return false;
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var sabActive = system.AnyActive;
                if (sabActive) return false;
                if (role.FlashTimer() != 0) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;

                role.TimeRemaining = CustomGameOptions.GrenadeDuration;
                role.StartFlash();
                
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.FlashGrenade, SendOption.Reliable, -1);
                writer.Write((byte)role.Player.PlayerId);
                writer.Write((byte)role.flashedPlayers.Count);
                foreach (var player in role.flashedPlayers)
                {
                    writer.Write(player.PlayerId);
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}