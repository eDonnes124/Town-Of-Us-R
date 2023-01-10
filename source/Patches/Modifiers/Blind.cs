using HarmonyLib;

namespace TownOfUs.Modifiers
{
    public class Blind
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Blind))
                {
                    try
                    {
                        __instance.ReportButton.gameObject.SetActive(false);
                    } catch {}
                }
            }
        }
    }
}