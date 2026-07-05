using HarmonyLib;
using TownOfUs.Modifiers;

namespace TownOfUs.Modifiers.UnderdogMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix()
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog)) return;
            var modifier = Modifier.GetModifier<Underdog>(PlayerControl.LocalPlayer);
            modifier.SetKillTimer();
        }
    }
}
