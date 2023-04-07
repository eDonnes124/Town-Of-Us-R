using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace TownOfUs.CustomOption
{
    public static class Patches
    {
        private static readonly string[] Menus = { "Game", "Crew", "Neutral", "Imposter", "Modifier" };
        public static Export ExportButton;
        public static Import ImportButton;
        public static Presets PresetButton;

        private static List<OptionBehaviour> CreateOptions(GameOptionsMenu __instance, MultiMenu type)
        {
            var options = new List<OptionBehaviour>();

            var togglePrefab = Object.FindObjectOfType<ToggleOption>();
            var numberPrefab = Object.FindObjectOfType<NumberOption>();
            var stringPrefab = Object.FindObjectOfType<StringOption>();

            if (type == MultiMenu.main)
            {
                if (ExportButton.Setting != null)
                {
                    ExportButton.Setting.gameObject.SetActive(true);
                    options.Add(ExportButton.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    ExportButton.Setting = toggle;
                    ExportButton.OptionCreated();
                    options.Add(toggle);
                }

                if (ImportButton.Setting != null)
                {
                    ImportButton.Setting.gameObject.SetActive(true);
                    options.Add(ImportButton.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    ImportButton.Setting = toggle;
                    ImportButton.OptionCreated();
                    options.Add(toggle);
                }

                if (PresetButton.Setting != null)
                {
                    PresetButton.Setting.gameObject.SetActive(true);
                    options.Add(PresetButton.Setting);
                }
                else
                {
                    var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    PresetButton.Setting = toggle;
                    PresetButton.OptionCreated();
                    options.Add(toggle);
                }
            }

            options.AddRange(__instance.Children);

            foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == type))
            {
                if (option.Setting != null)
                {
                    option.Setting.gameObject.SetActive(true);
                    options.Add(option.Setting);
                    continue;
                }

                switch (option.Type)
                {
                    case CustomOptionType.Number:
                        var number = Object.Instantiate(numberPrefab, numberPrefab.transform.parent);
                        option.Setting = number;
                        options.Add(number);
                        break;

                    case CustomOptionType.String:
                        var str = Object.Instantiate(stringPrefab, stringPrefab.transform.parent);
                        option.Setting = str;
                        options.Add(str);
                        break;

                    case CustomOptionType.Toggle:
                    case CustomOptionType.Nested:
                    case CustomOptionType.Button:
                    case CustomOptionType.Header:
                        var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent);

                        if (option.Type == CustomOptionType.Header)
                        {
                            toggle.transform.GetChild(1).gameObject.SetActive(false);
                            toggle.transform.GetChild(2).gameObject.SetActive(false);
                        }
                        else if (option.Type == CustomOptionType.Button || option.Type == CustomOptionType.Nested)
                        {
                            toggle.transform.GetChild(2).gameObject.SetActive(false);
                            toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                        }

                        option.Setting = toggle;
                        options.Add(toggle);
                        break;
                }

                option.OptionCreated();
            }

            return options;
        }

        private static bool OnEnable(OptionBehaviour opt)
        {
            if (opt == ExportButton.Setting)
            {
                ExportButton.OptionCreated();
                return false;
            }

            if (opt == ImportButton.Setting)
            {
                ImportButton.OptionCreated();
                return false;
            }

            if (opt == PresetButton.Setting)
            {
                PresetButton.OptionCreated();
                return false;
            }

            var customOption = CustomOption.AllOptions.Find(option => option.Setting == opt);
            // Works but may need to change to gameObject.name check

            if (customOption == null)
            {
                customOption = ExportButton.SlotButtons.Find(option => option.Setting == opt);

                if (customOption == null)
                {
                    customOption = ImportButton.SlotButtons.Find(option => option.Setting == opt);

                    if (customOption == null)
                    {
                        customOption = PresetButton.SlotButtons.Find(option => option.Setting == opt);

                        if (customOption == null)
                        {
                            customOption = CustomNestedOption.AllCancelButtons.Find(option => option.Setting == opt);

                            if (customOption == null)
                            {
                                customOption = CustomNestedOption.AllInternalOptions.Find(option => option.Setting == opt);

                                if (customOption == null)
                                    return true;
                            }
                        }
                    }
                }
            }

            customOption.OptionCreated();
            return false;
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        private class OptionsMenuBehaviour_Start
        {
            public static void Postfix(GameSettingMenu __instance)
            {
                var obj = __instance.RolesSettingsHightlight.gameObject.transform.parent.parent;
                var diff = (0.906f * Menus.Length) - 2;
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x - diff, obj.transform.localPosition.y, obj.transform.localPosition.z);
                __instance.GameSettingsHightlight.gameObject.transform.parent.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y,
                    obj.transform.localPosition.z);
                var menug = new List<GameObject>();
                var menugs = new List<SpriteRenderer>();

                for (var index = 0; index < Menus.Length; index++)
                {
                    var touSettings = Object.Instantiate(__instance.RegularGameSettings, __instance.RegularGameSettings.transform.parent);
                    touSettings.SetActive(false);
                    touSettings.name = "TOUSettings" + Menus[index];

                    var gameGroup = touSettings.transform.FindChild("GameGroup");
                    var title = gameGroup?.FindChild("Text");

                    if (title != null)
                    {
                        title.GetComponent<TextTranslatorTMP>().Destroy();
                        title.GetComponent<TMPro.TextMeshPro>().m_text = $"Town Of Us {Menus[index]} Settings";
                    }

                    var sliderInner = gameGroup?.FindChild("SliderInner");

                    if (sliderInner != null)
                        sliderInner.GetComponent<GameOptionsMenu>().name = $"Tou{Menus[index]}OptionsMenu";

                    var ourSettingsButton = Object.Instantiate(obj.gameObject, obj.transform.parent);
                    ourSettingsButton.transform.localPosition = new Vector3(obj.localPosition.x + (0.906f * (index + 1)), obj.localPosition.y, obj.localPosition.z);
                    ourSettingsButton.name = $"TOU{Menus[index]}tab";
                    var hatButton = ourSettingsButton.transform.GetChild(0); //TODO:  change to FindChild I guess to be sure
                    var hatIcon = hatButton.GetChild(0);
                    var tabBackground = hatButton.GetChild(1);

                    var renderer = hatIcon.GetComponent<SpriteRenderer>();
                    renderer.sprite = GetSettingSprite(index);
                    var touSettingsHighlight = tabBackground.GetComponent<SpriteRenderer>();
                    menug.Add(touSettings);
                    menugs.Add(touSettingsHighlight);

                    var passiveButton = tabBackground.GetComponent<PassiveButton>();
                    passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    passiveButton.OnClick.AddListener(ToggleButton(__instance, menug, menugs, index + 2));

                    //fix for scrollbar (bug in among us)
                    touSettings.GetComponentInChildren<Scrollbar>().parent = touSettings.GetComponentInChildren<Scroller>();
                }

                var passiveButton2 = __instance.GameSettingsHightlight.GetComponent<PassiveButton>();
                passiveButton2.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                passiveButton2.OnClick.AddListener(ToggleButton(__instance, menug, menugs, 0));
                passiveButton2 = __instance.RolesSettingsHightlight.GetComponent<PassiveButton>();
                passiveButton2.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                passiveButton2.OnClick.AddListener(ToggleButton(__instance, menug, menugs, 1));

                __instance.RegularGameSettings.GetComponentInChildren<Scrollbar>().parent = __instance.RegularGameSettings.GetComponentInChildren<Scroller>();

                try
                {
                    __instance.RolesSettings.GetComponentInChildren<Scrollbar>().parent = __instance.RolesSettings.GetComponentInChildren<Scroller>();
                } catch {}
            }

            static Sprite GetSettingSprite(int index)
            {
                return index switch
                {
                    0 => TownOfUs.SettingsButtonSprite,
                    1 => TownOfUs.CrewSettingsButtonSprite,
                    2 => TownOfUs.NeutralSettingsButtonSprite,
                    3 => TownOfUs.ImposterSettingsButtonSprite,
                    4 => TownOfUs.ModifierSettingsButtonSprite,
                    _ => TownOfUs.SettingsButtonSprite,
                };
            }
        }

        public static System.Action ToggleButton(GameSettingMenu settingMenu, List<GameObject> TouSettings, List<SpriteRenderer> highlight, int id)
        {
            return new System.Action(() =>
            {
                settingMenu.RegularGameSettings.SetActive(id == 0);
                settingMenu.GameSettingsHightlight.enabled = id == 0;
                settingMenu.RolesSettings.gameObject.SetActive(id == 1);
                settingMenu.RolesSettingsHightlight.enabled = id == 1;

                foreach (GameObject g in TouSettings)
                {
                    g.SetActive(id == TouSettings.IndexOf(g) + 2);
                    highlight[TouSettings.IndexOf(g)].enabled = id == TouSettings.IndexOf(g) + 2;
                }
            });
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        private class GameOptionsMenu_Start
        {
            public static bool Prefix(GameOptionsMenu __instance)
            {
                for (int index = 0; index < Menus.Length; index++)
                {
                    if (__instance.name == $"Tou{Menus[index]}OptionsMenu")
                    {
                        __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(System.Array.Empty<OptionBehaviour>());
                        var childeren = new Transform[__instance.gameObject.transform.childCount];

                        for (int k = 0; k < childeren.Length; k++)
                            childeren[k] = __instance.gameObject.transform.GetChild(k); //TODO: Make a better fix for this for example caching the options or creating it ourself.

                        var startOption = __instance.gameObject.transform.GetChild(0);
                        var customOptions = CreateOptions(__instance, (MultiMenu)index);
                        var y = startOption.localPosition.y;
                        var x = startOption.localPosition.x;
                        var z = startOption.localPosition.z;

                        for (int k = 0; k < childeren.Length; k++)
                            childeren[k].gameObject.Destroy();

                        var i = 0;

                        foreach (var option in customOptions)
                            option.transform.localPosition = new Vector3(x, y - (i++ * 0.5f), z);

                        __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(customOptions.ToArray());
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        private class GameOptionsMenu_Update
        {
            public static void Postfix(GameOptionsMenu __instance)
            {
                if (__instance.Children == null || __instance.Children.Length == 0)
                    return;

                var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
                var s = __instance.Children.Length == 1;
                var (x, z) = (__instance.Children[s ? 0 : 1].transform.localPosition.x, __instance.Children[s ? 0 : 1].transform.localPosition.z);
                var i = 0;

                foreach (var option in __instance.Children)
                    option.transform.localPosition = new Vector3(x, y - (i++ * 0.5f), z);

                try
                {
                    var commonTasks = __instance.Children.FirstOrDefault(x => x.name == "NumCommonTasks")?.TryCast<NumberOption>();

                    if (commonTasks != null)
                        commonTasks.ValidRange = new FloatRange(0f, 4f);

                    var shortTasks = __instance.Children.FirstOrDefault(x => x.name == "NumShortTasks")?.TryCast<NumberOption>();

                    if (shortTasks != null)
                        shortTasks.ValidRange = new FloatRange(0f, 26f);

                    var longTasks = __instance.Children.FirstOrDefault(x => x.name == "NumLongTasks")?.TryCast<NumberOption>();

                    if (longTasks != null)
                        longTasks.ValidRange = new FloatRange(0f, 15f);
                } catch {}
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        private static class ToggleOption_OnEnable
        {
            private static bool Prefix(ToggleOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        private static class NumberOption_OnEnable
        {
            private static bool Prefix(NumberOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
        private static class StringOption_OnEnable
        {
            private static bool Prefix(StringOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        private class ToggleButtonPatch
        {
            public static bool Prefix(ToggleOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                // Works but may need to change to gameObject.name check
                if (option is CustomToggleOption toggle)
                {
                    toggle.Toggle();
                    return false;
                }

                if (option is CustomNestedOption nested)
                {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    nested.ToDo();
                    return false;
                }

                if (__instance == ExportButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    ExportButton.Do();
                    return false;
                }

                if (__instance == ImportButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    ImportButton.Do();
                    return false;
                }

                if (__instance == PresetButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    PresetButton.Do();
                    return false;
                }

                if (option is CustomHeaderOption) return false;

                CustomOption option2 = ExportButton.SlotButtons.Find(option => option.Setting == __instance);
                if (option2 is CustomButtonOption button)
                {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    button.Do();
                    return false;
                }

                CustomOption option3 = ImportButton.SlotButtons.Find(option => option.Setting == __instance);
                if (option3 is CustomButtonOption button2)
                {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    button2.Do();
                    return false;
                }

                var option4 = PresetButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option4 is CustomButtonOption button3)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button3.Do();
                    return false;
                }

                var option5 = CustomNestedOption.AllCancelButtons.Find(option => option.Setting == __instance);

                if (option5 is CustomButtonOption button4)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button4.Do();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        private class NumberOptionPatchIncrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option =
                    CustomOption.AllOptions.Find(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomNumberOption number)
                {
                    number.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
        private class NumberOptionPatchDecrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option =
                    CustomOption.AllOptions.Find(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomNumberOption number)
                {
                    number.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
        private class StringOptionPatchIncrease
        {
            public static bool Prefix(StringOption __instance)
            {
                var option =
                    CustomOption.AllOptions.Find(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomStringOption str)
                {
                    str.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
        private class StringOptionPatchDecrease
        {
            public static bool Prefix(StringOption __instance)
            {
                var option =
                    CustomOption.AllOptions.Find(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomStringOption str)
                {
                    str.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        private class PlayerControlPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance || !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost)
                    return;

                Rpc.SendRpc();
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
        private class PlayerJoinPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance || !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost)
                    return;

                Rpc.SendRpc();
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        private class HudManagerUpdate
        {
            private const float
                MinX = -5.233334F /*-5.3F*/,
                OriginalY = 2.9F,
                MinY = 3F; // Differs to cause excess options to appear cut off to encourage scrolling

            private static Scroller Scroller;
            private static Vector3 LastPosition = new(MinX, MinY);

            public static void Prefix(HudManager __instance)
            {
                if (__instance.GameSettings?.transform == null)
                    return;

                // Scroller disabled
                if (!CustomOption.LobbyTextScroller)
                {
                    // Remove scroller if disabled late
                    if (Scroller != null)
                    {
                        __instance.GameSettings.transform.SetParent(Scroller.transform.parent);
                        __instance.GameSettings.transform.localPosition = new Vector3(MinX, OriginalY);

                        Object.Destroy(Scroller);
                    }

                    return;
                }

                CreateScroller(__instance);

                Scroller.gameObject.SetActive(__instance.GameSettings.gameObject.activeSelf);

                if (!Scroller.gameObject.active)
                    return;

                var rows = __instance.GameSettings.text.Count(c => c == '\n');
                var maxY = Mathf.Max(MinY, (rows * 0.081f) + ((rows - 38) * 0.081f));
                Scroller.ContentYBounds = new FloatRange(MinY, maxY);

                // Prevent scrolling when the player is interacting with a menu
                if (PlayerControl.LocalPlayer?.CanMove != true)
                {
                    __instance.GameSettings.transform.localPosition = LastPosition;
                    return;
                }

                if (__instance.GameSettings.transform.localPosition.x != MinX || __instance.GameSettings.transform.localPosition.y < MinY)
                    return;

                LastPosition = __instance.GameSettings.transform.localPosition;
            }

            private static void CreateScroller(HudManager __instance)
            {
                if (Scroller != null) return;

                Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
                Scroller.transform.SetParent(__instance.GameSettings.transform.parent);
                Scroller.gameObject.layer = 5;

                Scroller.transform.localScale = Vector3.one;
                Scroller.allowX = false;
                Scroller.allowY = true;
                Scroller.active = true;
                Scroller.velocity = new Vector2(0, 0);
                Scroller.ScrollbarYBounds = new FloatRange(0, 0);
                Scroller.ContentXBounds = new FloatRange(MinX, MinX);
                Scroller.enabled = true;

                Scroller.Inner = __instance.GameSettings.transform;
                __instance.GameSettings.transform.SetParent(Scroller.transform);
            }
        }
    }
}