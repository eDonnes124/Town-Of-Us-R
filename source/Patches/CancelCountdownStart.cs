using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using TownOfUs.CustomOption;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    class CancelCountdownStart
    {
        private static PassiveButton CancelStartButton;

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        [HarmonyPrefix]
        public static void PrefixStart(GameStartManager __instance)
        {
            CancelStartButton = Object.Instantiate(__instance.StartButton, __instance.transform);
            CancelStartButton.name = "CancelButton";
            
            var cancelLabel = CancelStartButton.buttonText;
            cancelLabel.gameObject.GetComponent<TextTranslatorTMP>()?.OnDestroy();
            cancelLabel.text = "Cancel";
            
            var cancelButtonInactiveRenderer = CancelStartButton.inactiveSprites.GetComponent<SpriteRenderer>();
            cancelButtonInactiveRenderer.color = new(0.8f, 0f, 0f, 1f);
            
            var cancelButtonActiveRenderer = CancelStartButton.activeSprites.GetComponent<SpriteRenderer>();
            cancelButtonActiveRenderer.color = Color.red;
            
            var cancelButtonInactiveShine = CancelStartButton.inactiveSprites.transform.Find("Shine");
            
            if (cancelButtonInactiveShine)
                cancelButtonInactiveShine.gameObject.SetActive(false);

            CancelStartButton.activeTextColor = CancelStartButton.inactiveTextColor = Color.white;
            
            CancelStartButton.OnClick = new();
            CancelStartButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                if (__instance.countDownTimer < 4f) __instance.ResetStartState();
            }));
            CancelStartButton.gameObject.SetActive(false);
            __instance.MinPlayers = 2;
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        [HarmonyPrefix]
        public static void PrefixUpdate(GameStartManager __instance)
        {
            if (__instance == null || !AmongUsClient.Instance.AmHost) return;

            CancelStartButton.gameObject.SetActive(__instance.startState is GameStartManager.StartingStates.Countdown);

            var startTexttransform = __instance.GameStartText.transform;
            if (startTexttransform.localPosition.y != 2f)
            {
                startTexttransform.localPosition = new Vector3(startTexttransform.localPosition.x, 2f, startTexttransform.localPosition.z);
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.ResetStartState))]
        [HarmonyPrefix]
        public static void Prefix(GameStartManager __instance)
        {
            if (__instance?.startState is GameStartManager.StartingStates.Countdown)
            {
                SoundManager.Instance.StopSound(__instance.gameStartSound);
                if (AmongUsClient.Instance.AmHost)
                {
                    RandomMap.Reset();
                    GameManager.Instance.LogicOptions.SyncOptions();
                    Coroutines.Start(Rpc.SendRpc());
                }
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.SetStartCounter))]
        [HarmonyPrefix]
        public static void Prefix(GameStartManager __instance, sbyte sec)
        {
            if (sec == -1)
            {
                SoundManager.Instance.StopSound(__instance.gameStartSound);
            }
        }
    }
}
