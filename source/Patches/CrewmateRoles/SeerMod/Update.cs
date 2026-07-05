using HarmonyLib;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Modifiers;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Seer seer)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var roleType = Utils.GetRole(player);
                    switch (roleType)
                    {
                        default:
                            if ((player.Is(Faction.Crewmates) && (StartImitate.ImitatingPlayers.Contains(player.PlayerId) || !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy)))) ||
                            ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy)) && !CustomGameOptions.CrewKillingRed) ||
                            (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                            (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                            (player.Is(Faction.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                            {
                                state.NameText.color = Color.green;
                            }
                            else if (player.Is(Faction.Impostors) && CustomGameOptions.TraitorColourSwap)
                            {
                                var imp = Role.GetRole(player);
                                if (imp.formerRole != RoleEnum.None)
                                {
                                    if ((imp.formerRole == RoleEnum.Sheriff || imp.formerRole == RoleEnum.Vigilante ||
                                            imp.formerRole == RoleEnum.Veteran || imp.formerRole == RoleEnum.Hunter ||
                                            imp.formerRole == RoleEnum.Deputy) && CustomGameOptions.CrewKillingRed) state.NameText.color = Color.red;
                                    else state.NameText.color = Color.green;
                                }
                                else state.NameText.color = Color.red;
                            }
                            else
                            {
                                state.NameText.color = Color.red;
                            }
                            break;
                    }
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

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            var seer = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, seer);

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!seer.Investigated.Contains(player.PlayerId)) continue;
                    var roleType = Utils.GetRole(player);
                    switch (roleType)
                    {
                        default:
                            var colour = Color.red;
                            if ((player.Is(Faction.Crewmates) && (StartImitate.ImitatingPlayers.Contains(player.PlayerId) || !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy)))) ||
                                ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy)) && !CustomGameOptions.CrewKillingRed) ||
                                (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                                (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                                (player.Is(Faction.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                            {
                                colour = Color.green;
                            }
                            else if (player.Is(Faction.Impostors) && CustomGameOptions.TraitorColourSwap)
                            {
                                var imp = Role.GetRole(player);
                                if (imp.formerRole != RoleEnum.None)
                                {
                                    if ((imp.formerRole == RoleEnum.Sheriff || imp.formerRole == RoleEnum.Vigilante ||
                                        imp.formerRole == RoleEnum.Veteran || imp.formerRole == RoleEnum.Hunter ||
                                        imp.formerRole == RoleEnum.Deputy) && CustomGameOptions.CrewKillingRed) colour = Color.red;
                                    else colour = Color.green;
                                }
                            }

                            if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                            player.nameText().color = colour;

                            break;
                    }
                }
            }
        }
    }
}