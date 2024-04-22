/*using HarmonyLib;
using PowerTools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.CustomHats.Patches
{
    public class HatLoad
    {

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
        public static class HP_patch
        { 
            public static bool Prefix(HatParent __instance, int color) {
                if (!HatCache.hatViewDatas.ContainsKey(__instance.Hat.ProductId)) return true;
                __instance.UnloadAsset();
                __instance.PopulateFromViewData();
                __instance.SetMaterialColor(color);
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.PopulateFromViewData))]
        public static class PF_patch
        {
            public static bool Prefix(HatParent __instance)
            {
                if (!HatCache.hatViewDatas.ContainsKey(__instance.Hat.ProductId)) return true;
                __instance.UpdateMaterial();
                Sprite hat = HatCache.hatViewDatas[__instance.Hat.ProductId];

                SpriteAnimNodeSync spriteAnimNodeSync = __instance.SpriteSyncNode ?? __instance.GetComponent<SpriteAnimNodeSync>();
                if ((bool)(Object)spriteAnimNodeSync)
                    spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;
                if (__instance.Hat.InFront)
                {
                    __instance.BackLayer.enabled = false;
                    __instance.FrontLayer.enabled = true;
                    __instance.FrontLayer.sprite = hat;
                }
                else
                {
                    __instance.BackLayer.enabled = true;
                    __instance.FrontLayer.enabled = false;
                    __instance.FrontLayer.sprite = null;
                    __instance.BackLayer.sprite = hat;
                }
                if (!__instance.options.Initialized || !__instance.HideHat())
                    return false;
                __instance.FrontLayer.enabled = false;
                __instance.BackLayer.enabled = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetClimbAnim))]
        public static class PF_climb_patch
        {
            public static bool Prefix(HatParent __instance)
            {
                if (!HatCache.hatViewDatas.ContainsKey(__instance.Hat.ProductId)) return true;
                __instance.FrontLayer.sprite = null;
                return false;
            }
        }
    }
}
*/