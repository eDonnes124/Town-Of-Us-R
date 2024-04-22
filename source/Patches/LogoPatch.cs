using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class LogoPatch
    {
        private static Sprite Sprite => TownOfUs.ToUBanner;
        static void Postfix(PingTracker __instance)
        {
            var touLogo = new GameObject("bannerLogo_TownOfUs");
            touLogo.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            var renderer = touLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;


            var position = touLogo.AddComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(-0.2f, 1.5f, 8f);
            position.Alignment = AspectPosition.EdgeAlignments.Top;

            position.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
            {
                position.AdjustPosition();
            })));


            var scaler = touLogo.AddComponent<AspectScaledAsset>();
            var renderers = new Il2CppSystem.Collections.Generic.List<SpriteRenderer>();
            renderers.Add(renderer);

            scaler.spritesToScale = renderers;
            scaler.aspectPosition = position;

            touLogo.transform.SetParent(GameObject.Find("RightPanel").transform);
        }
    }
}