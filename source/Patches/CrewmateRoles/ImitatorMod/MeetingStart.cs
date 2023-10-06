using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
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
