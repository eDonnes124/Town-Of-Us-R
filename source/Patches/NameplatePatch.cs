using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace TownOfUs
{
    public static class NameplatePatch
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
        public static class NameplateCosmetics
        {
            public static void Postfix(PlayerVoteArea __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo)
            {
                if (CustomGameOptions.WhiteNameplates)
                {
                    __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").Image;
                }

                if (CustomGameOptions.DisableLevels)
                {
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
        public static class NameplatePreview
        {
            public static void Postfix(PlayerVoteArea __instance, [HarmonyArgument(0)] string plateId)
            {
                if (CustomGameOptions.WhiteNameplates)
                {
                    __instance.Background.sprite = DestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").Image;
                }
                
                if (CustomGameOptions.DisableLevels)
                {
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
                    __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
                }
            }
        }
    }
}