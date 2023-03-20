using HarmonyLib;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            Utils.ResetCustomTimers();
        }
    }
}