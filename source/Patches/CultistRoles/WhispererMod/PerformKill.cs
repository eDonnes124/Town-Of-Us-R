using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;

namespace TownOfUs.CultistRoles.WhispererMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class Whisper
    {
        public static bool Prefix(AbilityButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);
            if (__instance.isCoolingDown) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.WhisperTimer() != 0) return false;

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            Vector2 truePosition = role.Player.GetTruePosition();
            var closestPlayers = Utils.GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius, false);
            if (role.PlayerConversion.Count == 0) role.PlayerConversion = role.GetPlayers();
            var oldStats = role.PlayerConversion;
            role.PlayerConversion = new List<(PlayerControl, int)>();
            foreach (var conversionRate in oldStats)
            {
                var player = conversionRate.Item1;
                var stats = conversionRate.Item2;
                if (closestPlayers.Contains(player))
                {
                    stats -= role.WhisperConversion;
                }
                if (!player.Data.IsDead) role.PlayerConversion.Add((player, stats));
            }
            role.WhisperCount += 1;
            role.LastWhispered = DateTime.UtcNow;
            CheckConversion(role);
            return false;
        }

        public static void CheckConversion(Whisperer role)
        {
            var removals = new List<(PlayerControl, int)>();
            foreach (var playerConversion in role.PlayerConversion)
            {
                if (playerConversion.Item2 <= 0)
                {
                    Utils.Convert(playerConversion.Item1);
                    role.ConversionCount += 1;
                    role.WhisperConversion -= CustomGameOptions.DecreasedPercentagePerConversion;
                    if (role.WhisperConversion < 5) role.WhisperConversion = 5;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.Convert, SendOption.Reliable, -1);
                    writer.Write(playerConversion.Item1.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    removals.Add(playerConversion);
                }
            }
            foreach (var removal in removals) role.PlayerConversion.Remove(removal);
            removals.Clear();
            return;
        }
    }
}