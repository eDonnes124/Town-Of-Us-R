using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;

namespace TownOfUs.CultistRoles.WhispererMod
{

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ColorNameUpdate
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.CultistSeer)
                    || player.Is(RoleEnum.Survivor)) state.NameText.color = new Color(0f, 1f, 1f, 1f);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer)) return;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            foreach (var stats in role.PlayerConversion)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (stats.Item1.PlayerId != state.TargetPlayerId) continue;
                    float color = stats.Item2 / 100f;
                    if (color <= 0) state.NameText.color = Patches.Colors.Impostor;
                    else state.NameText.color = new Color(1f, 1f, color, 1f);
                }
            }
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!(PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer) || PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer))) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.CultistSeer)
                    || player.Is(RoleEnum.Survivor)) player.nameText().color = new Color(0f, 1f, 1f, 1f);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer)) return;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            foreach (var stats in role.PlayerConversion)
            {
                float color = stats.Item2/100f;
                if (color <= 0) stats.Item1.nameText().color = Patches.Colors.Impostor;
                else stats.Item1.nameText().color = new Color(1f, 1f, color, 1f);
            }
        }
    }
}