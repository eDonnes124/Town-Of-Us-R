using HarmonyLib;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Modifiers;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            if (StartImitate.ImitatingPlayers.Contains(PlayerControl.LocalPlayer.PlayerId)) return;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = Role.GetRole(player);
                    if (player.Is(Faction.Impostors) && Role.GetRole(player).formerRole == RoleEnum.None)
                        state.NameText.color = Palette.ImpostorRed;
                    else if (player.Is(Faction.Impostors) && Role.GetRole(player).formerRole != RoleEnum.None && CustomGameOptions.SnitchSeesTraitor)
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(Faction.NeutralKilling) && CustomGameOptions.SnitchSeesNeutrals)
                        state.NameText.color = role.Color;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting) UpdateMeeting(MeetingHud.Instance);

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && Role.GetRole(player).formerRole == RoleEnum.None)
                    {
                        var colour = Palette.ImpostorRed;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    else if (player.Is(Faction.Impostors) && Role.GetRole(player).formerRole != RoleEnum.None && CustomGameOptions.SnitchSeesTraitor)
                    {
                        var colour = Palette.ImpostorRed;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    var playerRole = Role.GetRole(player);
                    if (playerRole.Faction == Faction.NeutralKilling && CustomGameOptions.SnitchSeesNeutrals)
                    {
                        var colour = playerRole.Color;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                }
            }
        }
    }
}