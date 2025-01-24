using System.Linq;
using System;
using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;
using Il2CppSystem.Text;
using Reactor.Utilities;
using System.Collections;

namespace TownOfUs.CustomOption
{
    public static class Patches
    {
        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
        private class MoreTasks
        {
            public static void Postfix(GameOptionsMenu __instance)
            {
                if (__instance.gameObject.name == "GAME SETTINGS TAB")
                {
                    try
                    {
                        var commonTasks = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks).Cast<NumberOption>();
                        if (commonTasks != null) commonTasks.ValidRange = new FloatRange(0f, 4f);

                        var shortTasks = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks).Cast<NumberOption>();
                        if (shortTasks != null) shortTasks.ValidRange = new FloatRange(0f, 26f);

                        var longTasks = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks).Cast<NumberOption>();
                        if (longTasks != null) longTasks.ValidRange = new FloatRange(0f, 15f);
                    }
                    catch
                    {

                    }
                }
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
        class ChangeTab
        {
            public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
            {
                if (previewOnly) return;
                foreach (var tab in SettingsUpdate.Tabs) if (tab != null) tab.SetActive(false);
                foreach (var button in SettingsUpdate.Buttons) button.SelectButton(false);
                if (tabNum > 2)
                {
                    tabNum -= 3;
                    SettingsUpdate.Tabs[tabNum].SetActive(true);
                    if (tabNum >= 5)
                    {
                        var tab = SettingsUpdate.Tabs[tabNum].GetComponent<GameOptionsMenu>();
                        tab.settingsContainer.DestroyChildren();
                        var files = Directory.GetFiles(Application.persistentDataPath, "*.txt").Select(x => Path.GetFileNameWithoutExtension(x).Split('/')[^1].Split('\\')[^1]).ToList();
                        float num = 1.5f;
                        if (tabNum == 6)
                        {
                            SettingsUpdate.SpawnExternalButton(__instance, tab, ref num, "Save To New File", () => SettingsUpdate.ExportSlot(__instance));
                            foreach (var file in files)
                                SettingsUpdate.SpawnExternalButton(__instance, tab, ref num, file, () => SettingsUpdate.ExportSlot(__instance, file));
                        }
                        else
                        {
                            foreach (var file in files)
                                SettingsUpdate.SpawnExternalButton(__instance, tab, ref num, file, () => SettingsUpdate.ImportSlot(__instance, file));
                        }
                        SettingsUpdate.SpawnExternalButton(__instance, tab, ref num, "Return", () => Coroutines.Start(TabPatches.ChangeTab(__instance, 3)));
                    }
                    if (tabNum > 4) return;
                    SettingsUpdate.Buttons[tabNum].SelectButton(true);

                    __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p =>
                    {
                        foreach (CustomOption option in CustomOption.AllOptions)
                        {
                            if (option.Type == CustomOptionType.Number)
                            {
                                var number = option.Setting.Cast<NumberOption>();
                                number.TitleText.text = option.Name;
                                if (number.TitleText.text.StartsWith("<color="))
                                    number.TitleText.fontSize = 3f;
                                else if (number.TitleText.text.Length > 20)
                                    number.TitleText.fontSize = 2.25f;
                                else if (number.TitleText.text.Length > 40)
                                    number.TitleText.fontSize = 2f;
                                else number.TitleText.fontSize = 2.75f;
                            }

                            else if (option.Type == CustomOptionType.Toggle)
                            {
                                var tgl = option.Setting.Cast<ToggleOption>();
                                tgl.TitleText.text = option.Name;
                                if (tgl.TitleText.text.Length > 20)
                                    tgl.TitleText.fontSize = 2.25f;
                                else if (tgl.TitleText.text.Length > 40)
                                    tgl.TitleText.fontSize = 2f;
                                else tgl.TitleText.fontSize = 2.75f;
                            }

                            else if (option.Type == CustomOptionType.String)
                            {
                                var playerCount = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                                if (option.Name.StartsWith("Slot "))
                                {
                                    try
                                    {
                                        int slotNumber = int.Parse(option.Name[5..]);
                                        if (slotNumber > GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers) continue;
                                    }
                                    catch { }
                                }

                                var str = option.Setting.Cast<StringOption>();
                                str.TitleText.text = option.Name;
                                if (str.TitleText.text.Length > 20)
                                    str.TitleText.fontSize = 2.25f;
                                else if (str.TitleText.text.Length > 40)
                                    str.TitleText.fontSize = 2f;
                                else str.TitleText.fontSize = 2.75f;
                            }
                        }
                    })));
                }
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
        private class CloseSettings
        {
            public static void Prefix(GameSettingMenu __instance)
            {
                LobbyInfoPane.Instance.EditButton.gameObject.SetActive(true);
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        private class SettingsUpdate
        {
            public static List<PassiveButton> Buttons = new List<PassiveButton>();
            public static List<GameObject> Tabs = new List<GameObject>();
            public static void Postfix(GameSettingMenu __instance)
            {
                LobbyInfoPane.Instance.EditButton.gameObject.SetActive(false);
                Buttons.ForEach(x => x?.Destroy());
                Tabs.ForEach(x => x?.Destroy());
                Buttons = new List<PassiveButton>();
                Tabs = new List<GameObject>();

                if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

                GameObject.Find("What Is This?")?.Destroy();
                GameObject.Find("RoleSettingsButton")?.Destroy();
                GameObject.Find("GamePresetButton")?.Destroy();
                __instance.ChangeTab(1, false);

                var settingsButton = GameObject.Find("GameSettingsButton");
                settingsButton.transform.localPosition += new Vector3(0f, 2f, 0f);
                settingsButton.transform.localScale *= 0.9f;

                CreateSettings(__instance, 3, "ModSettings", "Mod Settings", settingsButton, MultiMenu.main);
                CreateSettings(__instance, 4, "CrewSettings", "Crewmate Settings", settingsButton, MultiMenu.crewmate);
                CreateSettings(__instance, 5, "NeutralSettings", "Neutral Settings", settingsButton, MultiMenu.neutral);
                CreateSettings(__instance, 6, "ImpSettings", "Impostor Settings", settingsButton, MultiMenu.imposter);
                CreateSettings(__instance, 7, "ModifierSettings", "Modifier Settings", settingsButton, MultiMenu.modifiers);
                CreateSettings(__instance, 8, "ImportSettings", "Import Settings", settingsButton, MultiMenu.external);
                CreateSettings(__instance, 9, "ExportSettings", "Export Settings", settingsButton, MultiMenu.external);
            }

            internal static TextMeshPro SpawnExternalButton(GameSettingMenu __instance, GameOptionsMenu tabOptions, ref float num, string text, Action onClick)
            {
                const float scaleX = 7f;
                var baseButton = __instance.GameSettingsTab.checkboxOrigin.transform.GetChild(1);
                var baseText = __instance.GameSettingsTab.checkboxOrigin.transform.GetChild(0);

                var exportButtonGO = GameObject.Instantiate(baseButton, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                exportButtonGO.name = text;
                exportButtonGO.transform.localPosition = new Vector3(1f, num, -2f);
                exportButtonGO.GetComponent<BoxCollider2D>().offset = Vector2.zero;
                exportButtonGO.name = text.Replace(" ", "");

                var prevColliderSize = exportButtonGO.GetComponent<BoxCollider2D>().size;
                prevColliderSize.x *= scaleX;
                exportButtonGO.GetComponent<BoxCollider2D>().size = prevColliderSize;

                exportButtonGO.transform.GetChild(2).gameObject.DestroyImmediate();
                var exportButton = exportButtonGO.GetComponent<PassiveButton>();
                exportButton.ClickMask = tabOptions.ButtonClickMask;
                exportButton.OnClick.RemoveAllListeners();
                exportButton.OnClick.AddListener(onClick);

                var exportButtonTextGO = GameObject.Instantiate(baseText, exportButtonGO);
                exportButtonTextGO.transform.localPosition = new Vector3(0, 0, -3f);
                exportButtonTextGO.GetComponent<RectTransform>().SetSize(prevColliderSize.x, prevColliderSize.y);
                var exportButtonText = exportButtonTextGO.GetComponent<TextMeshPro>();
                exportButtonText.alignment = TextAlignmentOptions.Center;
                exportButtonText.SetText(text);

                SpriteRenderer[] componentsInChildren = exportButtonGO.GetComponentsInChildren<SpriteRenderer>(true);
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, 20);
                    componentsInChildren[i].transform.localPosition = new Vector3(0, 0, -1);
                    var prevSpriteSize = componentsInChildren[i].size;
                    prevSpriteSize.x *= scaleX;
                    componentsInChildren[i].size = prevSpriteSize;
                }
                TextMeshPro[] componentsInChildren2 = exportButtonGO.GetComponentsInChildren<TextMeshPro>(true);
                foreach (TextMeshPro obj in componentsInChildren2)
                {
                    obj.fontMaterial.SetFloat("_StencilComp", 3f);
                    obj.fontMaterial.SetFloat("_Stencil", 20);
                }

                num -= 0.6f;
                return exportButtonText;
            }

            public static TextMeshPro ImportText;
            public static TextMeshPro ExportText;

            public static void CreateSettings(GameSettingMenu __instance, int target, string name, string text, GameObject settingsButton, MultiMenu menu)
            {
                var panel = GameObject.Find("LeftPanel");
                var button = GameObject.Find(name);
                if (button == null && menu != MultiMenu.external)
                {
                    button = GameObject.Instantiate(settingsButton, panel.transform);
                    button.transform.localPosition += new Vector3(0f, -0.55f * target + 1.1f, 0f);
                    button.name = name;
                    __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p => { button.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = text; })));
                    var passiveButton = button.GetComponent<PassiveButton>();
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener((System.Action)(() =>
                    {
                        __instance.ChangeTab(target, false);
                    }));
                    passiveButton.SelectButton(false);
                    Buttons.Add(passiveButton);
                }

                var settingsTab = GameObject.Find("GAME SETTINGS TAB");
                Tabs.RemoveAll(x => x == null);
                var tab = GameObject.Instantiate(settingsTab, settingsTab.transform.parent);
                tab.name = name;
                var tabOptions = tab.GetComponent<GameOptionsMenu>();
                foreach (var child in tabOptions.Children) child.Destroy();
                tabOptions.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
                tabOptions.Children.Clear();
                var options = CustomOption.AllOptions.Where(x => x.Menu == menu).ToList();

                if (target < 8)
                {
                    float num = 1.5f;

                    if (target == 3)
                    {
                        ImportText = SpawnExternalButton(__instance, tabOptions, ref num, "Load Custom Settings", () => Coroutines.Start(TabPatches.ChangeTab(__instance, 8)));
                        ExportText = SpawnExternalButton(__instance, tabOptions, ref num, "Save Custom Settings", () => Coroutines.Start(TabPatches.ChangeTab(__instance, 9)));
                    }

                    foreach (CustomOption option in options)
                    {
                        if (option.Type == CustomOptionType.Header)
                        {
                            CategoryHeaderMasked header = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(tabOptions.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                            header.SetHeader(StringNames.ImpostorsCategory, 20);
                            header.Title.text = option.Name;
                            header.transform.localScale = Vector3.one * 0.65f;
                            header.transform.localPosition = new Vector3(-0.9f, num, -2f);
                            num -= 0.625f;
                            continue;
                        }

                        else if (option.Type == CustomOptionType.Number)
                        {
                            OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<NumberOption>(tabOptions.numberOptionOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                            optionBehaviour.transform.localPosition = new Vector3(0.95f, num, -2f);
                            optionBehaviour.SetClickMask(tabOptions.ButtonClickMask);
                            SpriteRenderer[] components = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                            for (int i = 0; i < components.Length; i++) components[i].material.SetInt(PlayerMaterial.MaskLayer, 20);

                            var numberOption = optionBehaviour as NumberOption;
                            option.Setting = numberOption;

                            tabOptions.Children.Add(optionBehaviour);
                        }

                        else if (option.Type == CustomOptionType.Toggle)
                        {
                            OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<ToggleOption>(tabOptions.checkboxOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                            optionBehaviour.transform.localPosition = new Vector3(0.95f, num, -2f);
                            optionBehaviour.SetClickMask(tabOptions.ButtonClickMask);
                            SpriteRenderer[] components = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                            for (int i = 0; i < components.Length; i++) components[i].material.SetInt(PlayerMaterial.MaskLayer, 20);

                            var toggleOption = optionBehaviour as ToggleOption;
                            option.Setting = toggleOption;

                            tabOptions.Children.Add(optionBehaviour);
                        }

                        else if (option.Type == CustomOptionType.String)
                        {
                            var playerCount = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                            if (option.Name.StartsWith("Slot "))
                            {
                                try
                                {
                                    int slotNumber = int.Parse(option.Name[5..]);
                                    if (slotNumber > GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers) continue;
                                }
                                catch { }
                            }

                            OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<StringOption>(tabOptions.stringOptionOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                            optionBehaviour.transform.localPosition = new Vector3(0.95f, num, -2f);
                            optionBehaviour.SetClickMask(tabOptions.ButtonClickMask);
                            SpriteRenderer[] components = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                            for (int i = 0; i < components.Length; i++) components[i].material.SetInt(PlayerMaterial.MaskLayer, 20);

                            var stringOption = optionBehaviour as StringOption;
                            option.Setting = stringOption;

                            tabOptions.Children.Add(optionBehaviour);
                        }

                        num -= 0.45f;
                        tabOptions.scrollBar.SetYBoundsMax(-num - 1.65f);
                        option.OptionCreated();
                    }
                }

                for (int i = 0; i < tabOptions.Children.Count; i++)
                {
                    OptionBehaviour optionBehaviour = tabOptions.Children[i];
                    if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost) optionBehaviour.SetAsPlayer();
                }

                Tabs.Add(tab);
                tab.SetActive(false);
            }

            public static void ImportSlot(GameSettingMenu __instance, string preset)
            {
                System.Console.WriteLine(preset);

                string text;

                try
                {
                    var path = Path.Combine(Application.persistentDataPath, $"{preset}.txt");
                    text = File.ReadAllText(path);
                }
                catch
                {
                    Coroutines.Start(TabPatches.Flash(__instance, ImportText, Color.red));
                    return;
                }

                var splitText = text.Split("\n").ToList();

                while (splitText.Count > 0)
                {
                    var name = splitText[0].Trim();
                    splitText.RemoveAt(0);
                    var option = CustomOption.AllOptions.FirstOrDefault(o => o.Name.Equals(name, StringComparison.Ordinal));
                    if (option == null)
                    {
                        try
                        {
                            splitText.RemoveAt(0);
                        }
                        catch
                        {
                        }
                        continue;
                    }

                    var value = splitText[0];
                    splitText.RemoveAt(0);
                    switch (option.Type)
                    {
                        case CustomOptionType.Number:
                            option.Set(float.Parse(value), false);
                            break;
                        case CustomOptionType.Toggle:
                            option.Set(bool.Parse(value), false);
                            break;
                        case CustomOptionType.String:
                            option.Set(int.Parse(value), false);
                            break;
                    }
                }

                Rpc.SendRpc();

                Coroutines.Start(TabPatches.Flash(__instance, ImportText, Color.green));
            }

            public static void ExportSlot(GameSettingMenu __instance)
            {
                System.Console.WriteLine("Exporting settings");

                var builder = new StringBuilder();
                foreach (var option in CustomOption.AllOptions)
                {
                    if (option.Type is CustomOptionType.Button or CustomOptionType.Header) continue;
                    builder.AppendLine(option.Name);
                    builder.AppendLine(option.Value.ToString());
                }

                var text = Path.Combine(Application.persistentDataPath, "Saved Settings 1.txt");
                var i = 1;

                while (File.Exists(text))
                {
                    i++;
                    text = Path.Combine(Application.persistentDataPath, $"Saved Settings {i}.txt");
                }

                try
                {
                    File.WriteAllText(text, builder.ToString());
                    Coroutines.Start(TabPatches.Flash(__instance, ExportText, Color.green));
                }
                catch
                {
                    Coroutines.Start(TabPatches.Flash(__instance, ExportText, Color.red));
                }
            }

            public static void ExportSlot(GameSettingMenu __instance, string preset)
            {
                System.Console.WriteLine($"Exporting settings to {preset}");

                var builder = new StringBuilder();
                foreach (var option in CustomOption.AllOptions)
                {
                    if (option.Type is CustomOptionType.Button or CustomOptionType.Header) continue;
                    builder.AppendLine(option.Name);
                    builder.AppendLine($"{option.Value}");
                }

                try
                {
                    var path = Path.Combine(Application.persistentDataPath, $"{preset}.txt");
                    File.WriteAllText(path, builder.ToString());
                    Coroutines.Start(TabPatches.Flash(__instance, ExportText, Color.green));
                }
                catch
                {
                    Coroutines.Start(TabPatches.Flash(__instance, ExportText, Color.red));
                }
            }
        }

        class TabPatches
        {
            public static IEnumerator ChangeTab(GameSettingMenu __instance, int tab)
            {
                yield return new WaitForSeconds(0.1f);
                __instance.ChangeTab(tab, false);
            }
            public static IEnumerator Flash(GameSettingMenu __instance, TextMeshPro buttonText, Color colour)
            {
                yield return new WaitForSeconds(0.1f);
                __instance.ChangeTab(3, false);
                buttonText.color = colour;
                yield return new WaitForSeconds(0.5f);
                buttonText.color = Color.white;
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
        class SetTabPane
        {
            public static bool Prefix(LobbyViewSettingsPane __instance)
            {
                if ((int)__instance.currentTab < 6)
                {
                    ChangeTabPane.Postfix(__instance, __instance.currentTab);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
        class ChangeTabPane
        {
            public static void Postfix(LobbyViewSettingsPane __instance, StringNames category)
            {
                int tab = (int)category;

                foreach (var button in SettingsAwake.Buttons) button.SelectButton(false);
                if (tab > 5) return;
                __instance.taskTabButton.SelectButton(false);

                if (tab > 0)
                {
                    tab -= 1;
                    SettingsAwake.Buttons[tab].SelectButton(true);
                    SettingsAwake.AddSettings(__instance, SettingsAwake.ButtonTypes[tab]);
                }
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
        class UpdatePane
        {
            public static void Postfix(LobbyViewSettingsPane __instance)
            {
                if (SettingsAwake.Buttons.Count == 0) SettingsAwake.Postfix(__instance);
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
        class SettingsAwake
        {
            public static List<PassiveButton> Buttons = new List<PassiveButton>();
            public static List<MultiMenu> ButtonTypes = new List<MultiMenu>();

            public static void Postfix(LobbyViewSettingsPane __instance)
            {
                Buttons.ForEach(x => x?.Destroy());
                Buttons.Clear();
                ButtonTypes.Clear();

                if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

                GameObject.Find("RolesTabs")?.Destroy();
                var overview = GameObject.Find("OverviewTab");
                overview.transform.localScale += new Vector3(-0.4f, 0f, 0f);
                overview.transform.localPosition += new Vector3(-1f, 0f, 0f);

                CreateButton(__instance, 1, "ModTab", "Mod Settings", MultiMenu.main, overview);
                CreateButton(__instance, 2, "CrewmateTab", "Crewmate Settings", MultiMenu.crewmate, overview);
                CreateButton(__instance, 3, "NeutralTab", "Neutral Settings", MultiMenu.neutral, overview);
                CreateButton(__instance, 4, "ImpostorTab", "Impostor Settings", MultiMenu.imposter, overview);
                CreateButton(__instance, 5, "ModifierTab", "Modifier Settings", MultiMenu.modifiers, overview);
            }

            public static void CreateButton(LobbyViewSettingsPane __instance, int target, string name, string text, MultiMenu menu, GameObject overview)
            {
                var tab = GameObject.Find(name);
                if (tab == null)
                {
                    tab = GameObject.Instantiate(overview, overview.transform.parent);
                    tab.transform.localPosition += new Vector3(2.05f, 0f, 0f) * target;
                    tab.name = name;
                    __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p => { tab.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = text; })));
                    var pTab = tab.GetComponent<PassiveButton>();
                    pTab.OnClick.RemoveAllListeners();
                    pTab.OnClick.AddListener((System.Action)(() => {
                        __instance.ChangeTab((StringNames)target);
                    }));
                    pTab.SelectButton(false);
                    Buttons.Add(pTab);
                    ButtonTypes.Add(menu);
                }
            }

            public static void AddSettings(LobbyViewSettingsPane __instance, MultiMenu menu)
            {
                var options = CustomOption.AllOptions.Where(x => x.Menu == menu).ToList();

                float num = 1.5f;
                int headingCount = 0;
                int settingsThisHeader = 0;
                int settingRowCount = 0;

                foreach (var option in options)
                {
                    if (option.Type == CustomOptionType.Header)
                    {
                        if (settingsThisHeader % 2 != 0) num -= 0.85f;
                        CategoryHeaderMasked header = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                        header.SetHeader(StringNames.ImpostorsCategory, 61);
                        header.Title.text = option.Name;
                        header.transform.SetParent(__instance.settingsContainer);
                        header.transform.localScale = Vector3.one;
                        header.transform.localPosition = new Vector3(-9.8f, num, -2f);
                        __instance.settingsInfo.Add(header.gameObject);
                        num -= 1f;
                        headingCount += 1;
                        settingsThisHeader = 0;
                        continue;
                    }

                    else
                    {
                        var playerCount = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                        if (option.Name.StartsWith("Slot "))
                        {
                            try
                            {
                                int slotNumber = int.Parse(option.Name[5..]);
                                if (slotNumber > GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers) continue;
                            }
                            catch { }
                        }

                        ViewSettingsInfoPanel panel = UnityEngine.Object.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
                        panel.transform.SetParent(__instance.settingsContainer);
                        panel.transform.localScale = Vector3.one;
                        if (settingsThisHeader % 2 != 0)
                        {
                            panel.transform.localPosition = new Vector3(-3f, num, -2f);
                            num -= 0.85f;
                        }
                        else
                        {
                            settingRowCount += 1;
                            panel.transform.localPosition = new Vector3(-9f, num, -2f);
                        }
                        settingsThisHeader += 1;
                        panel.SetInfo(StringNames.ImpostorsCategory, option.ToString(), 61);
                        panel.titleText.text = option.Name;
                        __instance.settingsInfo.Add(panel.gameObject);
                    }
                }

                float spacing = (headingCount * 1f + settingRowCount * 0.85f + 2f) / (headingCount + settingRowCount);
                __instance.scrollBar.CalculateAndSetYBounds((float)(__instance.settingsInfo.Count + headingCount + settingRowCount), 4f, 6f, spacing);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        private class PlayerControlPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance ||
                    !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost) return;

                Rpc.SendRpc();
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
        private class PlayerJoinPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance ||
                    !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost) return;

                Rpc.SendRpc();
            }
        }


        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        private class ToggleButtonPatch
        {
            public static bool Prefix(ToggleOption __instance)
            {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomToggleOption toggle)
                {
                    toggle.Toggle();
                    return false;
                }
                if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek || __instance.boolOptionName == BoolOptionNames.VisualTasks ||
                    __instance.boolOptionName == BoolOptionNames.AnonymousVotes || __instance.boolOptionName == BoolOptionNames.ConfirmImpostor) return true;
                return false;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        private class NumberOptionPatchIncrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
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
                    CustomOption.AllOptions.FirstOrDefault(option =>
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
                    CustomOption.AllOptions.FirstOrDefault(option =>
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
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomStringOption str)
                {
                    str.Decrease();
                    return false;
                }

                return true;
            }
        }
    }
}