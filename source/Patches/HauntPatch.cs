using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
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
            var modifier = Modifier.GetModifier(__instance.HauntTarget);

            __instance.FilterText.text = modifier != null ? $"{role.Name} - {modifier.Name}"
                                                          : $"{role.Name}";
            return false;
            
        }
    }
}