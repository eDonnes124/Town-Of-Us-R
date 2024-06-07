using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;

namespace TownOfUs.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (role.ObserveTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.LastObservedPlayer = role.ClosestPlayer;
            }
            if (interact[0] == true)
            {
                role.LastObserved = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastObserved = DateTime.UtcNow;
                role.LastObserved = role.LastObserved.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ObserveCooldown);
                return false;
            }
            else if (interact[5] == true)
            {
                role.LastObserved = DateTime.UtcNow;
                role.LastObserved = role.LastObserved.AddSeconds(CustomGameOptions.ProtectAbsorbCd - CustomGameOptions.ObserveCooldown);
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}