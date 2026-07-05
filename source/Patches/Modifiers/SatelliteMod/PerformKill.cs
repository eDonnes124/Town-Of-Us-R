using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Modifiers;

namespace TownOfUs.Modifiers.SatelliteMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Satellite)) return true;

            var role = Modifier.GetModifier<Satellite>(PlayerControl.LocalPlayer);
            if (__instance != role.DetectButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.DetectUsed) return false;
            if (role.StartTimer() > 0) return false;
            if (!__instance.enabled) return false;

            role.DetectUsed = true;

            Coroutines.Start(role.Detect());

            return false;
        }
    }
}