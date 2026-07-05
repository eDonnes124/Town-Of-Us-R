using TownOfUs.Roles;
using TownOfUs.Modifiers;
using HarmonyLib;
using AmongUs.GameOptions;

namespace TownOfUs
{
    [HarmonyPatch]

    internal sealed class Hauntpatch
    {
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
        [HarmonyPrefix]

        public static bool Prefix(HauntMenuMinigame __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
            var role = Role.GetRole(__instance.HauntTarget);
            var modifiers = Modifier.GetModifiers(__instance.HauntTarget);
            if (modifiers.Length == 0) __instance.FilterText.text = role.Name;
            else
            {
                string modifierText = " (";
                foreach (var modifier in modifiers)
                {
                    if (modifierText != " (") modifierText += ", ";
                    modifierText += modifier.Name;
                }
                modifierText += ")";
                __instance.FilterText.text = role.Name + modifierText;
            }
            return false;
        }
    }
}