using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {

        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();
            var host = GameData.Instance?.GetHost();

            __instance.text.text =
                "<size=2><color=#00FF00FF>TownOfUs v" + TownOfUs.VersionString + "</color>" + TownOfUs.VersionTag + "\n" +
                $"Ping: {AmongUsClient.Instance?.Ping}ms\n" +
                (!MeetingHud.Instance
                    ? "<color=#00FF00FF>Modded By: Donners &</color>\n" +
                    "<color=#00FF00FF>MyDragonBreath</color>\n" : "") +
                (!MeetingHud.Instance ? "<color=#A970FF>Twitch Fork: Xslash58</color>\n" : "") +
                (AmongUsClient.Instance?.GameState != InnerNet.InnerNetClient.GameStates.Started
                    ? "<color=#00FF00FF>Formerly: Slushiegoose & Polus.gg</color>" +
                     (CustomGameOptions.HideHost ? "" : $"\nHost: {host.PlayerName}") : "") +
                    "</size>";
        }
    }
}
