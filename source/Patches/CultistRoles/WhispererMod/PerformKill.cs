using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace TownOfUs.CultistRoles.WhispererMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Whisper
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);
            if (__instance == role.WhisperButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.WhisperTimer() != 0) return false;

                var flag2 = role.WhisperButton.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                Vector2 truePosition = role.Player.GetTruePosition();
                var closestPlayers = Utils.GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius, false);
                if (role.PlayerConversion.Count == 0) role.PlayerConversion = role.GetPlayers();
                var oldStats = role.PlayerConversion;
                role.PlayerConversion = [];
                foreach (var conversionRate in oldStats)
                {
                    var player = conversionRate.Player;
                    var stats = conversionRate.UnconvertableChance;
                    if (closestPlayers.Contains(player))
                    {
                        stats -= role.WhisperConversion;
                    }
                    if (!player.Data.IsDead) role.PlayerConversion.Add(new(player, stats));
                }
                role.WhisperCount += 1;
                role.LastWhispered = DateTime.UtcNow;
                CheckConversion(role);
                return false;
            }
            return true;
        }

        public static void CheckConversion(Whisperer role)
        {
            var removals = new List<Whisperer.ConversionData>();
            foreach (var playerConversion in role.PlayerConversion)
            {
                if (playerConversion.UnconvertableChance <= 0)
                {
                    Utils.Convert(playerConversion.Player);
                    role.ConversionCount += 1;
                    role.WhisperConversion -= CustomGameOptions.DecreasedPercentagePerConversion;
                    if (role.WhisperConversion < 5) role.WhisperConversion = 5;

                    Utils.Rpc(CustomRPC.Convert, playerConversion.Player.PlayerId);
                    removals.Add(playerConversion);
                }
            }
            foreach (var removal in removals) role.PlayerConversion.Remove(removal);
            removals.Clear();
            return;
        }
    }
}