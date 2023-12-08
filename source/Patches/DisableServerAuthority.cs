using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches;

internal static class DisableServerAuthority
{

[HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
[HarmonyPostfix]
    public static void  GetBroadcastVersionPatch(ref int __result)
    {
        if (AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame) return;
        Debug.Log("Sending the DisableServerAuthority flag");
        __result += 25;
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
    [HarmonyPrefix]
    public static bool IsVersionModdedPatch(ref bool __result)
    {
        __result = true;
        Debug.Log("Detecting a Modded of verison Among Us");
        return false;
    }
  }  