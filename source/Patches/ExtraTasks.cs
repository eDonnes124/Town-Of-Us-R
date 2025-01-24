using AmongUs.GameOptions;
using HarmonyLib;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(ShipStatus))]
    public class ShipStatusPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
        public static void Postfix(LogicGameFlowNormal __instance, ref bool __result)
        {
            __result = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool Prefix(ShipStatus __instance)
        {
            var commonTask = __instance.CommonTasks.Count;
            var normalTask = __instance.ShortTasks.Count;
            var longTask = __instance.LongTasks.Count;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks > commonTask) GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = commonTask;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks > normalTask) GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = normalTask;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks > longTask) GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = longTask;
            return true;
        }
    }

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    public class GetAdjustedImposters
    {
        public static bool Prefix(IGameOptions __instance, ref int __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;

            var players = GameData.Instance.PlayerCount;
            var impostors = 0;
            List<RoleOptions> impBuckets = [RoleOptions.ImpConceal, RoleOptions.ImpKilling, RoleOptions.ImpSupport, RoleOptions.ImpCommon, RoleOptions.ImpRandom];
            List<RoleOptions> buckets = [CustomGameOptions.Slot1, CustomGameOptions.Slot2, CustomGameOptions.Slot3, CustomGameOptions.Slot4];
            var anySlots = 0;

            if (players > 4) buckets.Add(CustomGameOptions.Slot5);
            if (players > 5) buckets.Add(CustomGameOptions.Slot6);
            if (players > 6) buckets.Add(CustomGameOptions.Slot7);
            if (players > 7) buckets.Add(CustomGameOptions.Slot8);
            if (players > 8) buckets.Add(CustomGameOptions.Slot9);
            if (players > 9) buckets.Add(CustomGameOptions.Slot10);
            if (players > 10) buckets.Add(CustomGameOptions.Slot11);
            if (players > 11) buckets.Add(CustomGameOptions.Slot12);
            if (players > 12) buckets.Add(CustomGameOptions.Slot13);
            if (players > 13) buckets.Add(CustomGameOptions.Slot14);
            if (players > 14) buckets.Add(CustomGameOptions.Slot15);

            foreach (var roleOption in buckets)
            {
                if (impBuckets.Contains(roleOption)) impostors += 1;
                else if (roleOption == RoleOptions.Any) anySlots += 1;
            }

            int impProbability = (int)Math.Floor((double)players / anySlots * 5 / 3);
            for (int i = 0; i < anySlots; i++)
            {
                var random = Random.RandomRangeInt(0, 100);
                if (random < impProbability) impostors += 1;
                impProbability += 3;
            }

            if (players < 7 || impostors == 0) impostors = 1;
            else if (players < 10 && impostors > 2) impostors = 2;
            else if (players < 14 && impostors > 3) impostors = 3;
            else if (players < 19 && impostors > 4) impostors = 4;
            else if (impostors > 5) impostors = 5;
            __result = impostors;
            return false;
        }
    }
}
