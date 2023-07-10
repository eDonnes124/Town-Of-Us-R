using HarmonyLib;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowRoundOneShield
    {
        public static Color ShieldColor = Color.green;
        public static string DiedFirst = "";
        public static PlayerControl FirstRoundShielded = null;

        public static void Postfix(HudManager __instance)
        {
            if (FirstRoundShielded != null && !FirstRoundShielded.Data.Disconnected)
            {
                FirstRoundShielded.myRend().material.SetColor("_VisorColor", ShieldColor);
                FirstRoundShielded.myRend().material.SetFloat("_Outline", 1f);
                FirstRoundShielded.myRend().material.SetColor("_OutlineColor", ShieldColor);
            }
        }
    }
}