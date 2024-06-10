using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator))
            {
                // RPC how many uses your ability had left so the Imitator's role knows the accurate count
                List<RoleEnum> LimitedUsesRoles = new List<RoleEnum>()
                {
                    RoleEnum.Engineer,
                    RoleEnum.Veteran,
                    RoleEnum.VampireHunter,
                    RoleEnum.Transporter,
                    RoleEnum.Trapper
                };
                RoleEnum playerRole = Role.GetRole(PlayerControl.LocalPlayer).RoleType;
                if (PlayerControl.LocalPlayer.Data.IsDead && LimitedUsesRoles.Contains(playerRole) && !StartImitate.UpdatedUses)
                {
                    int newUses = 0;
                    if (playerRole == RoleEnum.Engineer)
                    {
                        newUses = ((Engineer)Role.GetRole(PlayerControl.LocalPlayer)).UsesLeft;
                    }
                    else if (playerRole == RoleEnum.Veteran)
                    {
                        newUses = ((Veteran)Role.GetRole(PlayerControl.LocalPlayer)).UsesLeft;
                    }
                    else if (playerRole == RoleEnum.VampireHunter)
                    {
                        newUses = ((VampireHunter)Role.GetRole(PlayerControl.LocalPlayer)).UsesLeft;
                    }
                    else if (playerRole == RoleEnum.Transporter)
                    {
                        newUses = ((Transporter)Role.GetRole(PlayerControl.LocalPlayer)).UsesLeft;
                    }
                    else if (playerRole == RoleEnum.Trapper)
                    {
                        newUses = ((Trapper)Role.GetRole(PlayerControl.LocalPlayer)).UsesLeft;
                    }
                    foreach(var role in Role.GetRoles(RoleEnum.Imitator))
                    {
                        // Only needed once per game
                        StartImitate.UpdatedUses = true;
                    }
                    Utils.Rpc(CustomRPC.UpdateImitator, PlayerControl.LocalPlayer.PlayerId, newUses);
                }
                return;
            }
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var imitatorRole = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitatorRole.trappedPlayers != null)
            {
                if (imitatorRole.trappedPlayers.Count == 0)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No players entered any of your traps");
                }
                else if (imitatorRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your traps");
                }
                else
                {
                    string message = "Roles caught in your trap:\n";
                    foreach (RoleEnum role in imitatorRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" {role},";
                    }
                    message.Remove(message.Length - 1, 1);
                    if (DestroyableSingleton<HudManager>.Instance)
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
                imitatorRole.trappedPlayers.Clear();
            }
            else if (imitatorRole.confessingPlayer != null)
            {
                var playerResults = MeetingStartOracle.PlayerReportFeedback(imitatorRole.confessingPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
        }
    }
}
