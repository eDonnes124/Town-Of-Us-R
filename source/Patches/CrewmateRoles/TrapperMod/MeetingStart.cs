using HarmonyLib;
using System;
using System.Linq;
using TownOfUs.Patches.Localization;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TrapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return;
            var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
            if (trapperRole.trappedPlayers.Count == 0)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, LocalizationManager.Instance.GetString("NoPlayersEnteredTraps"));
            }
            else if (trapperRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, LocalizationManager.Instance.GetString("NotEnoughPlayersTriggeredTraps"));
            }
            else
            {
                string message = $"{LocalizationManager.Instance.GetString("RolesCaughtInYourTrap")}:\n";
                foreach (RoleEnum role in trapperRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                {
                    message += $" {role},";
                }
                message.Remove(message.Length - 1, 1);
                if (DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }
    }
}
