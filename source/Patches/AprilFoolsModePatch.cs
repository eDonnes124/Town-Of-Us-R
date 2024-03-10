// Part Of the code from Pietrodjaowjao

using HarmonyLib;
using System;
using static CosmeticsLayer;
using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Patches;

[HarmonyPatch(typeof(LongBoiPlayerBody))]
public static class LongBoiPatches
{
    [HarmonyPatch(nameof(LongBoiPlayerBody.Awake))]
    [HarmonyPrefix]
    public static bool LongBoyAwake_Patch(LongBoiPlayerBody __instance)
    {
        __instance.cosmeticLayer.OnSetBodyAsGhost += (Action)__instance.SetPoolableGhost;
        __instance.cosmeticLayer.OnColorChange += (Action<int>)__instance.SetHeightFromColor;
        __instance.cosmeticLayer.OnCosmeticSet += (Action<string, int, CosmeticKind>)__instance.OnCosmeticSet;
        __instance.gameObject.layer = 8;
        return false;
    }

    [HarmonyPatch(nameof(LongBoiPlayerBody.Start))]
    [HarmonyPrefix]
    public static bool LongBoyStart_Patch(LongBoiPlayerBody __instance)
    {
        __instance.ShouldLongAround = true;
        if (__instance.hideCosmeticsQC)
        {
            __instance.cosmeticLayer.SetHatVisorVisible(false);
        }
        __instance.SetupNeckGrowth(false, true);
        if (__instance.isExiledPlayer)
        {
            ShipStatus instance = ShipStatus.Instance;
            if (instance == null || instance.Type != ShipStatus.MapType.Fungle)
            {
                __instance.cosmeticLayer.AdjustCosmeticRotations(-17.75f);
            }
        }
        if (!__instance.isPoolablePlayer)
        {
            __instance.cosmeticLayer.ValidateCosmetics();

            
        }
        return false;
    }

    [HarmonyPatch(nameof(LongBoiPlayerBody.SetHeighFromDistanceHnS))]
    [HarmonyPrefix]
    public static bool LongBoyNeckSize_Patch(LongBoiPlayerBody __instance, ref float distance)
    {
        __instance.targetHeight = distance / 10f + 0.5f;
        __instance.SetupNeckGrowth(true, true); //se quiser sim mano
        return false;
    }

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.CheckLongModeValidCosmetic))]
    [HarmonyPrefix]
    public static bool CheckLongMode_Patch(HatManager __instance, out bool __result, ref string cosmeticID, ref bool ignoreLongMode)
    {
        if (!ignoreLongMode)
        {
            List<CosmeticData>.Enumerator enumerator = __instance.longModeBlackList.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (string.Equals(enumerator.Current.ProdId, cosmeticID))
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }

            __result = true;
            return false;
        }
        __result = true;
        return false;
    }
}