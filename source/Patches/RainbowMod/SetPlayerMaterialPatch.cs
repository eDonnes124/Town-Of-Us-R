using HarmonyLib;
using UnityEngine;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
    public class SetPlayerMaterialPatch
    {
        public static bool Prefix([HarmonyArgument(0)] int colorId, [HarmonyArgument(1)] Renderer rend)
        {
            RainbowBehaviour r = rend.gameObject.GetComponent<RainbowBehaviour>() ?? rend.gameObject.AddComponent<RainbowBehaviour>();

            r.AddRend(rend, colorId);
            return !RainbowUtils.IsRainbow(colorId);
        }
    }

    [HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(Color), typeof(Renderer))]
    public class SetPlayerMaterialPatch2
    {
        public static bool Prefix([HarmonyArgument(1)] Renderer rend)
        {
            var r = rend.gameObject.GetComponent<RainbowBehaviour>() ?? rend.gameObject.AddComponent<RainbowBehaviour>();

            r.AddRend(rend, 0);
            return true;
        }
    }
}
