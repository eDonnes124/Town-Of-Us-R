using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (ShowRoundOneShield.FirstRoundShielded != null && !ShowRoundOneShield.FirstRoundShielded.Data.Disconnected)
            {
                ShowRoundOneShield.FirstRoundShielded.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                ShowRoundOneShield.FirstRoundShielded.myRend().material.SetFloat("_Outline", 0f);
                ShowRoundOneShield.FirstRoundShielded = null;
            }
        }
    }
}