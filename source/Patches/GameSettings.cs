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
        public static bool AllOptions;
        public static int SettingsPage = 0;

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static class GameOptionsDataPatch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                return typeof(GameOptionsData).GetMethods(typeof(string), typeof(int));
            }

            private static void Postfix(ref string __result)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                var builder = new StringBuilder(AllOptions ? __result : "");
                builder.AppendLine($"Currently Viewing Page ({(SettingsPage + 1)}/5)");
                builder.AppendLine("Press Tab To Change Pages");

                if (SettingsPage == 1)
                    builder.Append(new StringBuilder(__result));

                foreach (var option in CustomOption.CustomOption.AllOptions.Where(x => x.Menu == (MultiMenu)SettingsPage))
                {
                    if (option.Type == CustomOptionType.Button)
                        continue;

                    if (option.Type == CustomOptionType.Header)
                        builder.AppendLine($"\n{option.Name}");
                    else
                        builder.AppendLine($"    {option.Name}: {option}");
                }

                __result = builder.ToString();
                __result = $"<size=1.25>{__result}</size>";
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
            public static void Postfix(HudManager __instance)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (GameSettings.SettingsPage + 1 > 5)
                        GameSettings.SettingsPage = 0;
                    else
                        GameSettings.SettingsPage++;
                }
            }
        }
    }
}