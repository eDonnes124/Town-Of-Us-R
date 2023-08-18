using UnityEngine;
using HarmonyLib;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        private static float deltaTime;

        public static bool Started => AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;
        public static void Postfix(PingTracker __instance)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            var fps = Mathf.Ceil(1.0f / deltaTime);
            var host = GameData.Instance.GetHost();
            
          
            __instance.text.text = $"<size=80%>Ping: {AmongUsClient.Instance.Ping}ms FPS: {fps}\n" +
               $"<b><color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>v{TownOfUs.VersionString}</color></b>\n" +
                $"{(!MeetingHud.Instance ? $"<color=#0000FFFF>{TownOfUs.VersionString}</color>\n" : "")}" +
                $"{(!Started ? "<color=#C50000FF>By: eDonners & MyDragonBreath</color>\n" : "</size>")}" +
                "<color=#ff0000>FormerlyFrom</color> : <color=#ffc0cb>Polus.gg</color> Slushiegoose\n" + 
                // Idea from wichtwix see  pr/168
                $"HostedLobby : {host.PlayerName}";

        }

        // code from TOUReworked

        public static void Prefix(PingTracker __instance)
        {
            if (!__instance.GetComponentInChildren<SpriteRenderer>())
            {
                var logo = new GameObject("Logo") { layer = 5 };
                logo.AddComponent<SpriteRenderer>().sprite = TownOfUs.SettingsButtonSprite;
                logo.transform.SetParent(__instance.transform);
                logo.transform.localPosition = new(-1f, -0.5f, -1);
                logo.transform.localScale *= 0.5f;
            }
        }
    }
}
