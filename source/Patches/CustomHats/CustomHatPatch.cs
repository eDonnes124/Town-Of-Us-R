using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.CustomHats
{

    [HarmonyPatch]

    public static class HatsTab_OnEnable
    {
        public static int CurrentPage = 0;

        public static string LastHeader = string.Empty;
        
        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]

        public static bool Prefix(HatsTab __instance)
        {
            __instance.currentHat = HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat);
            var allHats = HatManager.Instance.GetUnlockedHats().ToImmutableList();

            if (HatCache.SortedHats == null)
            {
                var num = 0;
                HatCache.SortedHats = new(new PaddedComparer<string>("Vanilla", ""));
                foreach (var hat in allHats)
                {
                    if (!HatCache.SortedHats.ContainsKey(hat.StoreName)) HatCache.SortedHats[hat.StoreName] = [];
                    HatCache.SortedHats[hat.StoreName].Add(hat);

                    if (!HatCache.StoreNames.ContainsValue(hat.StoreName))
                    {
                        HatCache.StoreNames.Add(num, hat.StoreName);
                        num++;
                    }
                }
            }

            GenHats(__instance, CurrentPage);

            return false;
        }

        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.Update))]
        [HarmonyPrefix]

        public static void Update(HatsTab __instance)
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                CurrentPage--;
                CurrentPage = CurrentPage < 0 ? HatCache.StoreNames.Count - 1 : CurrentPage;
                GenHats(__instance, CurrentPage);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                CurrentPage++;
                CurrentPage = CurrentPage > HatCache.StoreNames.Count - 1 ? 0 : CurrentPage;
                GenHats(__instance, CurrentPage);
            }
        }

        public static void GenHats(HatsTab __instance, int page)
        {
            foreach (ColorChip instanceColorChip in __instance.ColorChips) instanceColorChip.gameObject.Destroy();
            __instance.ColorChips.Clear();

            if (LastHeader != string.Empty)
            {
                var header = GameObject.Find(LastHeader);
                if (header != null) header.Destroy();
            }

            var groupNameText = __instance.GetComponentInChildren<TextMeshPro>(false);
            int hatIdx = 0;
            var group = HatCache.SortedHats.Where(x => x.Key == HatCache.StoreNames[page]);
            foreach ((string groupName, List<HatData> hats) in group)
            {
                hatIdx = (hatIdx + 4) / 5 * 5;
                var text = Object.Instantiate(groupNameText, __instance.scroller.Inner);
                text.gameObject.transform.localScale = Vector3.one;
                text.GetComponent<TextTranslatorTMP>().Destroy();
                text.text = $"{groupName}\nPress F3 & F4 to cycle pages";
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 3f;
                text.fontSizeMax = 3f;
                text.fontSizeMin = 0f;
                LastHeader = text.name = $"{groupName} header";
                float xLerp = __instance.XRange.Lerp(0.5f);
                float yLerp = __instance.YStart - hatIdx / __instance.NumPerRow * __instance.YOffset;
                text.transform.localPosition = new Vector3(xLerp, yLerp, -1f);

                hatIdx += 5;
                foreach (var hat in hats.OrderBy(HatManager.Instance.allHats.IndexOf))
                {
                    float num = __instance.XRange.Lerp(hatIdx % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    float num2 = __instance.YStart - hatIdx / __instance.NumPerRow * __instance.YOffset;

                    var colorChip = Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
                    colorChip.gameObject.name = hat.ProductId;
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                    colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectHat(hat)));
                    colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                    colorChip.Inner.SetHat(hat, __instance.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
                    colorChip.transform.localPosition = new Vector3(num, num2, -1f);
                    colorChip.Inner.transform.localPosition = hat.ChipOffset + new Vector2(0f, -0.3f);
                    if (SubmergedCompatibility.Loaded)
                    {
                        colorChip.gameObject.transform.Find("HatParent").transform.localPosition = new Vector3(-0.1f, 0.05f, -2);
                    }
                    colorChip.Tag = hat;
                    __instance.ColorChips.Add(colorChip);
                    hatIdx += 1;
                }
            }

            __instance.scroller.ContentYBounds.max = -(__instance.YStart - (hatIdx + 1) / __instance.NumPerRow * __instance.YOffset) - 3f;
            __instance.currentHatIsEquipped = true;
        }
    }
}
