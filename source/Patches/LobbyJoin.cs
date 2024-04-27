using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]

    public sealed class LobbyJoin
    {
        static int GameId;

        static GameObject LobbyText;
        
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
        [HarmonyPostfix]

        public static void Postfix(InnerNetClient __instance)
        {
            GameId = __instance.GameId;
        }

        [HarmonyPatch(typeof(MMOnlineManager), nameof(MMOnlineManager.Start))]
        [HarmonyPostfix]

        public static void Postfix()
        {
            LobbyText = new("lobbycode");
            var comp = LobbyText.AddComponent<TMPro.TextMeshPro>();
            comp.fontSize = 2.5f;
            LobbyText.transform.localPosition = new(10.3f, -3.9f, 0);
            LobbyText.SetActive(true);
        }

        [HarmonyPatch(typeof(MMOnlineManager), nameof(MMOnlineManager.Update))]
        [HarmonyPostfix]

        public static void Postfix(MMOnlineManager __instance)
        {
            if (GameId == 0) return;
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                __instance.StartCoroutine_Auto(AmongUsClient.Instance.CoJoinOnlineGameFromCode(GameId));
            }
            if (LobbyText && LobbyText.GetComponent<TMPro.TextMeshPro>()) 
            {
                LobbyText.GetComponent<TMPro.TextMeshPro>().text = 
                    $"Prev Lobby: {GameCode.IntToGameName(GameId)}\nClick Tab key to attempt joining";
            }
        }
    }
}