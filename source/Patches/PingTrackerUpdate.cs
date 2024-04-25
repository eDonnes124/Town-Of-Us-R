using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        private static float deltaTime;

        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();
            var host = GameData.Instance?.GetHost();
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            var fps = Mathf.Ceil(1f / deltaTime);

            __instance.text.text =
                "<size=2><color=#00FF00FF>TownOfUs v" + TownOfUs.VersionString + "</color>" + TownOfUs.VersionTag + "\n" +
                $"Ping: {AmongUsClient.Instance?.Ping}ms\nFPS: {fps}\n" +
                (!MeetingHud.Instance
                    ? "<color=#00FF00FF>Modded By: Donners &</color>\n" +
                    "<color=#00FF00FF>MyDragonBreath</color>\n" : "") +
                (AmongUsClient.Instance?.GameState != InnerNet.InnerNetClient.GameStates.Started
                    ? "<color=#00FF00FF>Formerly: Slushiegoose & Polus.gg</color>\n" +
                     $"Host: {host?.PlayerName}" : "") +
                    "</size>";
        }
    }
}
