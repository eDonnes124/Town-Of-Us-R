using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Oracle);
            if (!flag) return true;
            var role = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.ConfessTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Confessor = role.ClosestPlayer;
                bool showsCorrectFaction = true;
                int faction = 1;
                if (role.Accuracy == 0f) showsCorrectFaction = false;
                else
                {
                    var num = UnityEngine.Random.RandomRangeInt(1, 101);
                    showsCorrectFaction = num <= role.Accuracy;
                }
                if (showsCorrectFaction)
                {
                    if (role.Confessor.Is(Faction.Crewmates)) faction = 0;
                    else if (role.Confessor.Is(Faction.Impostors)) faction = 2;
                }
                else
                {
                    var num = UnityEngine.Random.RandomRangeInt(0, 2);
                    if (role.Confessor.Is(Faction.Impostors)) faction = num;
                    else if (role.Confessor.Is(Faction.Crewmates)) faction = num + 1;
                    else if (num == 1) faction = 2;
                    else faction = 0;
                }
                if (faction == 0) role.RevealedFaction = Faction.Crewmates;
                else if (faction == 1) role.RevealedFaction = Faction.NeutralEvil;
                else role.RevealedFaction = Faction.Impostors;
                Utils.Rpc(CustomRPC.Confess, PlayerControl.LocalPlayer.PlayerId, role.Confessor.PlayerId, faction);
            }
            if (interact[0] == true)
            {
                role.LastConfessed = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastConfessed = DateTime.UtcNow;
                role.LastConfessed = role.LastConfessed.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ConfessCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
