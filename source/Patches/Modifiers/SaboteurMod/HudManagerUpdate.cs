using HarmonyLib;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Modifiers.SaboteurMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Saboteur)) return;

            var sab = Modifier.GetModifier<Saboteur>(PlayerControl.LocalPlayer);

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            if (system.AnyActive) system.Timer = 30f;
            else if (system.Timer > 30f - CustomGameOptions.ReducedSaboCd) system.Timer = 30f - CustomGameOptions.ReducedSaboCd;
        }
    }

    [HarmonyPatch(typeof(SabotageSystemType), nameof(SabotageSystemType.UpdateSystem))]
    public class SabotageSystemUpdate
    {
        public static void Prefix(SabotageSystemType __instance, ref PlayerControl player)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!player.Is(ModifierEnum.Saboteur)) return;

            if (__instance.Timer <= CustomGameOptions.ReducedSaboCd) __instance.Timer = 0f;
        }
    }

    [HarmonyPatch(typeof(SabotageSystemType), nameof(SabotageSystemType.Deserialize))]
    public class SabotageSystemDeserialize
    {
        public static void Prefix(SabotageSystemType __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (__instance.AnyActive) return;
            if (__instance.initialCooldown) return;

            foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Saboteur))
            {
                var sab = (Saboteur)modifier;
                sab.Timer = __instance.Timer;
            }
        }
        public static void Postfix(SabotageSystemType __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (__instance.AnyActive) return;
            if (__instance.initialCooldown) return;

            foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Saboteur))
            {
                var sab = (Saboteur)modifier;
                __instance.Timer = sab.Timer;
            }
        }
    }
}