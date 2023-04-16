using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

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
        
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.FixedUpdate))]
        [HarmonyPrefix]

        public static void prefix(HauntMenuMinigame __instance)
        {
            if (__instance.amClosing == Minigame.CloseState.Closing)
            {
                __instance.amClosing = Minigame.CloseState.None;
                return;
            }
            else if (__instance.amClosing == Minigame.CloseState.None)
            {
                __instance.amClosing = Minigame.CloseState.Closing;
            }
        }


    }
}