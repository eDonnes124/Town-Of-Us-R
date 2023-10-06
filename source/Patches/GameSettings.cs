using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.CustomOption;
using AmongUs.GameOptions;
using System.Linq;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class GameSettings
    {
        public static int SettingsPage = -1;

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static class GameOptionsDataPatch
        {
            private static void Postfix(ref string __result)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                var builder = new StringBuilder();
                builder.AppendLine("Press Tab To Change Page");
                builder.AppendLine($"Currently Viewing Page ({(SettingsPage + 2)}/6)");

                if (SettingsPage == -1)
                {
                    builder.Append("\nVanilla Settings\n");
                    builder.Append(new StringBuilder(__result));
                }
                else
                {
                    if (SettingsPage == 0) builder.AppendLine("\nGeneral Mod Settings");
                    else if (SettingsPage == 1) builder.AppendLine("\nCrewmate Settings");
                    else if (SettingsPage == 2) builder.AppendLine("\nNeutral Settings");
                    else if (SettingsPage == 3) builder.AppendLine("\nImpostor Settings");
                    else if (SettingsPage == 4) builder.AppendLine("\nModifier Settings");
                    var tobedisplayed = CustomOption.CustomOption.AllOptions.Where(x => x.Menu == (MultiMenu)SettingsPage).ToList();

                    foreach (var option in tobedisplayed)
                    {
                        var index = tobedisplayed.IndexOf(option);
                        if (option.Type == CustomOptionType.Button)
                            continue;

                        if (option.Type == CustomOptionType.Header)
                            builder.AppendLine($"\n{option.Name}");
                        else if (option.Type == CustomOptionType.Nested)
                        {
                            var nested = (CustomNestedOption)option;

                            //Nothing's better then a good ol' copy paste lmao
                            foreach (var option2 in nested.InternalOptions)
                            {
                                if (option2.Type == CustomOptionType.Button)
                                    continue;

                                if (option2.Type == CustomOptionType.Header)
                                    builder.AppendLine($"\n{option2.Name}");
                                else
                                    builder.AppendLine($"{(index == tobedisplayed.Count - 1 || tobedisplayed[index + 1].Type == CustomOptionType.Header ? "┗ " : "┣ ")}{option2.Name}: {option2}");
                            }
                        }
                        else
                            builder.AppendLine($"{(index == tobedisplayed.Count - 1 || tobedisplayed[index + 1].Type == CustomOptionType.Header ? "┗ " : "┣ ")}{option.Name}: {option}");
                    }
                }

                __result = $"<size=1.25>{builder.ToString()}</size>";
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update
        {
            public static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentInParent<Scroller>().ContentYBounds.max = (__instance.Children.Length - 6.5f) / 2;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class LobbyPatch
        {
            public static void Postfix()
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (SettingsPage > 3)
                        SettingsPage = -1;
                    else
                        SettingsPage++;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    SettingsPage = -1;

                if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    SettingsPage = 0;

                if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    SettingsPage = 1;

                if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                    SettingsPage = 2;

                if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                    SettingsPage = 3;

                if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                    SettingsPage = 4;
            }
        }
    }
}