using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUs
{
    [HarmonyPatch]

    public class HideVanilla
    {
        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        [HarmonyPrefix]

        public static bool Prefix(ref string __result, ref StringNames id)
        {
            switch (id)
            {
                case StringNames.EngineerRole:
                case StringNames.ScientistRole:
                case StringNames.GuardianAngelRole:
                case StringNames.ShapeshifterRole:
                case StringNames.RoleChanceAndQuantity:
                    __result = "";
                    return false;
            }
            return true;
        }
    }
}