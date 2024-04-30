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
        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]

        public static bool Prefix(HatsTab __instance)
        {
            __instance.currentHat = HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat);
            var allHats = HatManager.Instance.GetUnlockedHats().ToImmutableList();

            if (HatCache.SortedHats == null)
            {
                HatCache.SortedHats = new(new PaddedComparer<string>("Vanilla", ""));
                foreach (var hat in allHats)
                {
                    if (!HatCache.SortedHats.ContainsKey(hat.StoreName)) HatCache.SortedHats[hat.StoreName] = [];
                    HatCache.SortedHats[hat.StoreName].Add(hat);
                }
            }

            if (HatCache.Children.Count > 0)
            {
                HatCache.Children.Values.Do(x =>
                {
                    x.transform.SetParent(__instance.scroller.Inner);
                    x.gameObject.SetActive(true);
                });
                __instance.scroller.ContentYBounds.max = -(__instance.YStart - (HatCache.hatIdx + 1) / __instance.NumPerRow * __instance.YOffset) - 3f;
                __instance.currentHatIsEquipped = true;

                return false;
            }

            foreach (ColorChip instanceColorChip in __instance.ColorChips) instanceColorChip.gameObject.Destroy();
            __instance.ColorChips.Clear();

            var groupNameText = __instance.GetComponentInChildren<TextMeshPro>(false);
            int hatIdx = 0;
            foreach ((string groupName, List<HatData> hats) in HatCache.SortedHats)
            {
                hatIdx = (hatIdx + 4) / 5 * 5;
                var text = Object.Instantiate(groupNameText, __instance.scroller.Inner);
                text.gameObject.transform.localScale = Vector3.one;
                text.GetComponent<TextTranslatorTMP>().Destroy();
                text.text = groupName;
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 3f;
                text.fontSizeMax = 3f;
                text.fontSizeMin = 0f;
                text.name = $"{groupName} header";
                float xLerp = __instance.XRange.Lerp(0.5f);
                float yLerp = __instance.YStart - hatIdx / __instance.NumPerRow * __instance.YOffset;
                text.transform.localPosition = new Vector3(xLerp, yLerp, -1f);
                HatCache.Children.Add(groupName, text.transform);

                hatIdx += 5;
                foreach (var hat in hats.OrderBy(HatManager.Instance.allHats.IndexOf))
                {
                    float num = __instance.XRange.Lerp(hatIdx % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    float num2 = __instance.YStart - hatIdx / __instance.NumPerRow * __instance.YOffset;

                    var colorChip = Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
                    colorChip.gameObject.name = hat.ProductId;
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectHat(hat)));
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
                    HatCache.Children.Add(hat.ProductId, colorChip.gameObject.transform);
                    __instance.ColorChips.Add(colorChip);
                    hatIdx += 1;
                }
            }

            HatCache.hatIdx = hatIdx;
            __instance.scroller.ContentYBounds.max = -(__instance.YStart - (hatIdx + 1) / __instance.NumPerRow * __instance.YOffset) - 3f;
            __instance.currentHatIsEquipped = true;

            return false;
        }
    }

    [HarmonyPatch]

    public class HatsTab_OnDisable
    {
        [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.OnDisable))]
        [HarmonyPrefix]

        public static bool Prefix(InventoryTab __instance)
        {
            if (__instance is not HatsTab) return true;

            foreach (var transform in HatCache.Children.Values)
            {
                transform.SetParent(HatCache.ChipHolder.transform);
                transform.gameObject.SetActive(false);
            }

            __instance.ColorChips.Clear();
            return false;
        }

        [HarmonyPatch(typeof(PlayerCustomizationMenu), nameof(PlayerCustomizationMenu.OpenTab))]
        [HarmonyPrefix]

        public static void Prefix(ref InventoryTab tab)
        {
            if (tab is HatsTab hattab && hattab.ColorChips.Count == 0)
            {
                foreach (var transform in HatCache.Children.Values)
                {
                    if (transform.gameObject.GetComponent<ColorChip>())
                    {
                        hattab.ColorChips.Add(transform.gameObject.GetComponent<ColorChip>());
                    }
                }
            }
        }
    }
}
