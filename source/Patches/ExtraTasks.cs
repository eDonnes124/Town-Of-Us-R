using AmongUs.GameOptions;
using HarmonyLib;
using Random = UnityEngine.Random;

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
            if (CustomGameOptions.GameMode == GameMode.Cultist)
            {
                __result = 1;
                return false;
            }
            if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
            {
                var players = GameData.Instance.PlayerCount;

                var impostors = 1;
                var random = Random.RandomRangeInt(0, 100);
                if (players <= 6) impostors = 1;
                else if (players <= 7)
                {
                    if (random < 20) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 8)
                {
                    if (random < 40) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 9)
                {
                    if (random < 50) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 10)
                {
                    if (random < 60) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 11)
                {
                    if (random < 60) impostors = 2;
                    else if (random < 70) impostors = 3;
                    else impostors = 1;
                }
                else if (players <= 12)
                {
                    if (random < 60) impostors = 2;
                    else if (random < 80) impostors = 3;
                    else impostors = 1;
                }
                else if (players <= 13)
                {
                    if (random < 60) impostors = 2;
                    else if (random < 90) impostors = 3;
                    else impostors = 1;
                }
                else if (players <= 14)
                {
                    if (random < 50) impostors = 3;
                    else impostors = 2;
                }
                else
                {
                    if (random < 60) impostors = 3;
                    else if (random < 90) impostors = 2;
                    else impostors = 4;
                }
                __result = impostors;
                return false;
            }
            return true;
        }
    }
}
