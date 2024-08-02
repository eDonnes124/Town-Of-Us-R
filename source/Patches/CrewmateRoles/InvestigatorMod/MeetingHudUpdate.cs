using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = Role.GetRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Investigator) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Investigator)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = Utils.PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.InvestigatedPlayer = null;
                    continue;
                }
                if (role.InvestigatedPlayer.PlayerId == targetId) state.NameText.color = Color.cyan;
            }
        }
    }
}