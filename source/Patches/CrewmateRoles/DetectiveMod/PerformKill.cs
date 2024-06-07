using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.CrewmateRoles.MedicMod;
using Reactor.Utilities;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective)) return true;
            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];

            if (__instance == role.ExamineButton)
            {
                var flag2 = role.ExamineTimer() == 0f;
                if (!flag2) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (role.ClosestPlayer == null) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    if (role.DetectedKillers.Contains(role.ClosestPlayer.PlayerId) || (CustomGameOptions.CanDetectLastKiller && role.LastKiller == role.ClosestPlayer)) Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                if (interact[0] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ExamineCd);
                    return false;
                }
                else if (interact[5])
                {
                    role.LastExamined = DateTime.UtcNow;
                    role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectAbsorbCd - CustomGameOptions.ExamineCd);
                }
                else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                if (role.CurrentTarget == null)
                    return false;
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                foreach (var deadPlayer in Murder.KilledPlayers)
                {
                    if (deadPlayer.PlayerId == playerId) role.DetectedKillers.Add(deadPlayer.KillerId);
                }
                return false;
            }
        }
    }
}
