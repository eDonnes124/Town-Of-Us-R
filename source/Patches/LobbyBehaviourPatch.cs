using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches {
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch {
        [HarmonyPostfix]
        public static void Postfix() {
            // Fix Grenadier blind in lobby
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = false;

            // Fix Upside-down camera flip in lobby
            UnityEngine.Camera.main.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
        }
    }
}
