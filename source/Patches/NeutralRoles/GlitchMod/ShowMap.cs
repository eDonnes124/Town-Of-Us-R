using HarmonyLib;

namespace TownOfUs.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    internal class EngineerMapOpen
    {
        private static bool Prefix()
        {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch);
        }
    }
}