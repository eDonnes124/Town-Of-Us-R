using System;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public static class HatOffsetPatches
    {
        private static readonly Vector3 _hatOffset = new Vector3(-0.04f, 0.575f, 0f);

        [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.UpdateFromPlayerOutfit))]
        [HarmonyPrefix]
        public static void PoolablePlayer_UpdateFromPlayerOutfit_Prefix(PoolablePlayer __instance)
        {
            var offset = _hatOffset; offset.y = 0.575f;
            __instance.HatSlot.transform.localPosition = offset;
        }

        [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__18), nameof(IntroCutscene._CoBegin_d__18.MoveNext))]
        [HarmonyPrefix]
        public static void CoBegin(IntroCutscene._CoBegin_d__18 __instance)
        {
            if (__instance.__1__state == 0)
            {
                __instance.__4__this.PlayerPrefab.HatSlot.transform.localPosition = _hatOffset;
            }
        }

        [HarmonyPatch(typeof(EndGameManager._CoBegin_d__18), nameof(EndGameManager._CoBegin_d__18.MoveNext))]
        [HarmonyPrefix]
        public static void CoBegin(EndGameManager._CoBegin_d__18 __instance)
        {
            if (__instance.__1__state == 0)
            {
                __instance.__4__this.PlayerPrefab.HatSlot.transform.localPosition = _hatOffset;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetCosmetics))]
        [HarmonyPrefix]
        public static void SetCosmetics(ChatBubble __instance)
        {
            __instance.Player.HatSlot.transform.localPosition = _hatOffset;
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
        [HarmonyPostfix]
        public static void SetCosmetics(PlayerVoteArea __instance)
        {
            __instance.PlayerIcon.HatSlot.transform.localPosition = _hatOffset;
        }
    }
}