using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using InnerNet;

namespace TownOfUs.BanPatch;

public static class BanManager
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.CanBan))]
    internal static class InnerNetClientCanBanPatch
    {
        public static bool Prefix(InnerNetClient __instance, ref bool __result)
        {
            __result = __instance.AmHost;
            return false;
        }
    }
}