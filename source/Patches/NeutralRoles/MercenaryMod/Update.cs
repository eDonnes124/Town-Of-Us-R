using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Modifiers;
using UnityEngine;

namespace TownOfUs.NeutralRoles.MercenaryMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Mercenary merc)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!merc.Bribed.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var colour = Color.green;
                    if (player.Is(Faction.NeutralEvil) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Mercenary)) colour = Color.red;
                    state.NameText.color = colour;
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)

        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary)) return;
            var merc = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, merc);

            byte toRemove = 255;
            foreach (var guardId in merc.Guarded)
            {
                var guard = Utils.PlayerById(guardId);
                if (guard == null || guard.Data == null || guard.Data.IsDead || guard.Data.Disconnected)
                {
                    toRemove = guardId;
                    break;
                }
            }
            if (toRemove != 255)
            {
                merc.Guarded.Remove(toRemove);
            }

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!merc.Bribed.Contains(player.PlayerId)) continue;
                    var colour = Color.green;
                    if (player.Is(Faction.NeutralEvil) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Mercenary)) colour = Color.red;
                    if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                    player.nameText().color = colour;
                }
            }
        }
    }
}