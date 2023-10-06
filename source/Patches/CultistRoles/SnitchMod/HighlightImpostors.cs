using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;

namespace TownOfUs.CultistRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostor
    {
        public static void UpdateMeeting(MeetingHud __instance, CultistSnitch snitch)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (PlayerControl.LocalPlayer.Is(Faction.Impostors))
                    {
                        if (player != snitch.RevealedPlayer || PlayerControl.LocalPlayer == player) continue;
                        if (snitch.RevealedPlayer.Is(Faction.Impostors)) state.NameText.color = new Color(0.3f, 0.0f, 0.0f);
                        else state.NameText.color = new Color(0.8f, 0f, 0.6f);
                    }
                    else if (player == snitch.RevealedPlayer && PlayerControl.LocalPlayer != player) state.NameText.color = Palette.ImpostorRed;
                }
            }
        }
        public static void Postfix(HudManager __instance)
        {
            foreach (var snitch in Role.GetRoles(RoleEnum.CultistSnitch))
            {
                var role = (CultistSnitch)snitch;
                if (!role.CompletedTasks || role.Player.Data.IsDead) continue;
                if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance, role);
            }
        }
    }
}