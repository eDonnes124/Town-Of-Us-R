using HarmonyLib;

namespace TownOfUs
{
    [HarmonyPatch]

    public sealed class EmergencyCooldownPatch
    {
        public static double Time { get; set; }

        [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Begin))]
        [HarmonyPostfix]

        public static void Postfix(EmergencyMinigame __instance)
        {
            if (Time < CustomGameOptions.InitialCooldowns) __instance.ForceClose();
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]

        public static void Postfix()
        {
            if (!GameManager.Instance.GameHasStarted)
            {
                return;
            }

            if (Time < CustomGameOptions.InitialCooldowns) Time += UnityEngine.Time.fixedDeltaTime;
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        [HarmonyPostfix]

        public static void postfix()
        {
            Time = 0d;
        }
    }
}
