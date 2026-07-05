using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.CustomHats
{

    [HarmonyPatch]

    public static class HatsTab_OnEnable
    {
        public static int CurrentPage = 0;

        public static string LastHeader = string.Empty;

        private static IEnumerator loadRoutine;

        private static int hatIndex;

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
            var leftButtonElement = Object.Instantiate(PlayerCustomizationMenu.Instance.BackButton.gameObject, __instance.scroller.Inner);
            var leftSpriteRenderer = leftButtonElement.GetComponent<SpriteRenderer>();
            var leftButton = leftButtonElement.GetComponent<PassiveButton>();

            leftButtonElement.transform.localPosition = new Vector3(-1f, -1f, -1);
            leftSpriteRenderer.sprite = TownOfUs.NextSprite;
            leftSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            leftSpriteRenderer.flipX = true;

            leftButton.OnMouseOver = new UnityEvent();
            leftButton.OnMouseOver.AddListener((Action)(() => leftSpriteRenderer.sprite = TownOfUs.NextActiveSprite));
            leftButton.OnMouseOut = new UnityEvent();
            leftButton.OnMouseOut.AddListener((Action)(() => leftSpriteRenderer.sprite = TownOfUs.NextSprite));

            leftButton.OnClick = new Button.ButtonClickedEvent();
            leftButton.OnClick.AddListener((Action)(() => PreviousPage(__instance)));

            var rightButtonElement = Object.Instantiate(leftButtonElement, __instance.scroller.Inner);
            var rightSpriteRenderer = rightButtonElement.GetComponent<SpriteRenderer>();
            var rightButton = rightButtonElement.GetComponent<PassiveButton>();

            rightButtonElement.transform.localPosition = new Vector3(2.7f, -1f, -1);
            rightSpriteRenderer.sprite = TownOfUs.NextSprite;
            rightSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            rightSpriteRenderer.flipX = false;

            rightButton.OnMouseOver = new UnityEvent();
            rightButton.OnMouseOver.AddListener((Action)(() => rightSpriteRenderer.sprite = TownOfUs.NextActiveSprite));
            rightButton.OnMouseOut = new UnityEvent();
            rightButton.OnMouseOut.AddListener((Action)(() => rightSpriteRenderer.sprite = TownOfUs.NextSprite));

            rightButton.OnClick = new Button.ButtonClickedEvent();
            rightButton.OnClick.AddListener((Action)(() => NextPage(__instance)));

            return false;
        }

        private static void PreviousPage(HatsTab hatsTab)
        {
            CurrentPage--;
            CurrentPage = CurrentPage < 0 ? HatCache.StoreNames.Count - 1 : CurrentPage;
            GenHats(hatsTab, CurrentPage);
        }

        private static void NextPage(HatsTab hatsTab)
        {
            CurrentPage++;
            CurrentPage = CurrentPage > HatCache.StoreNames.Count - 1 ? 0 : CurrentPage;
            GenHats(hatsTab, CurrentPage);
        }

        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.Update))]
        [HarmonyPrefix]

        public static void Update(HatsTab __instance)
        {
            if (HatCache.SortedHats.Count == 0) return;

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousPage(__instance);
            }
            else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextPage(__instance);
            }
        }

        public static void GenHats(HatsTab __instance, int page)
        {
            if (loadRoutine != null) Coroutines.Stop(loadRoutine);

            hatIndex = 0;

            foreach (ColorChip instanceColorChip in __instance.ColorChips) instanceColorChip.gameObject.Destroy();
            __instance.ColorChips.Clear();

            if (LastHeader != string.Empty)
            {
                var header = GameObject.Find(LastHeader);
                if (header != null) header.Destroy();
            }

            var groupNameText = __instance.GetComponentInChildren<TextMeshPro>(false);
            var group = HatCache.SortedHats.Where(x => x.Key == HatCache.StoreNames[page]);
            foreach ((string groupName, List<HatData> hats) in group)
            {
                hatIndex = (hatIndex + 4) / 5 * 5;
                var text = Object.Instantiate(groupNameText, __instance.scroller.Inner);
                text.gameObject.transform.localScale = Vector3.one;
                text.GetComponent<TextTranslatorTMP>().Destroy();
                text.text = $"{groupName} ({page + 1} / {HatCache.StoreNames.Count})\nPress Ctrl & Tab to cycle pages";
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 3f;
                text.fontSizeMax = 3f;
                text.fontSizeMin = 0f;
                LastHeader = text.name = $"{groupName} header";
                float xLerp = __instance.XRange.Lerp(0.5f);
                float yLerp = __instance.YStart - hatIndex / __instance.NumPerRow * __instance.YOffset;
                text.transform.localPosition = new Vector3(xLerp, yLerp, -1f);

                hatIndex += 5;
                loadRoutine = Coroutines.Start(CoGenerateChips(__instance, hats));
            }

            __instance.scroller.ContentYBounds.max = -(__instance.YStart - (hatIndex + 1) / __instance.NumPerRow * __instance.YOffset) - 3f;
            __instance.currentHatIsEquipped = true;
        }

        private static IEnumerator CoGenerateChips(HatsTab __instance, List<HatData> hats)
        {
            __instance.scroller.ScrollToTop();
            var batchSize = 5;
            for (var i = 0; i < hats.Count; i += batchSize)
            {
                var batch = hats.Skip(i).Take(batchSize).ToList();

                foreach (var hat in batch.OrderBy(HatManager.Instance.allHats.IndexOf))
                {
                    var hatXposition = __instance.XRange.Lerp(hatIndex % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    var hatYposition = __instance.YStart - hatIndex / __instance.NumPerRow * __instance.YOffset;
                    GenerateColorChip(__instance, new Vector2(hatXposition, hatYposition), hat);
                    hatIndex += 1;
                    yield return null;
                }

                __instance.scroller.ContentYBounds.max = -(__instance.YStart - (hatIndex + 1) / __instance.NumPerRow * __instance.YOffset) - 3f;
                yield return new WaitForSeconds(0.01f);
            }
            __instance.currentHatIsEquipped = true;
            loadRoutine = null;
        }

        private static void GenerateColorChip(HatsTab __instance, Vector2 position, HatData hat)
        {
            var colorChip = Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
            colorChip.gameObject.name = hat.ProductId;
            colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
            colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectHat(hat)));
            colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
            colorChip.Inner.SetHat(hat, __instance.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            __instance.UpdateMaterials(colorChip.Inner.FrontLayer, hat);
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.SimpleUI);
            colorChip.transform.localPosition = new Vector3(position.x, position.y, -1f);
            colorChip.Inner.transform.localPosition = hat.ChipOffset + new Vector2(0f, -0.3f);
            colorChip.Tag = hat;
            __instance.ColorChips.Add(colorChip);
        }
    }
}
