using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.CrewmateRoles.MedicMod;
using Reactor.Utilities;
using AmongUs.GameOptions;
using System.Collections.Generic;
using TownOfUs.Extensions;

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
                if (role.InvestigatingScene == null) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    if (role.InvestigatedPlayers.Contains(role.ClosestPlayer.PlayerId))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        var deadPlayer = role.InvestigatingScene.DeadPlayer;
                        if (DestroyableSingleton<HudManager>.Instance)
                            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{role.ClosestPlayer.GetDefaultOutfit().PlayerName} was at the scene of {deadPlayer.GetDefaultOutfit().PlayerName}'s death!");
                    }
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
                else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                if (role.CurrentTarget == null)
                    return false;
                if (Vector2.Distance(role.CurrentTarget.gameObject.transform.position,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var player = role.CurrentTarget.DeadPlayer;
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                role.InvestigatingScene = role.CurrentTarget;
                role.InvestigatedPlayers.AddRange(role.CurrentTarget.ScenePlayers);
                return false;
            }
        }
    }
}
