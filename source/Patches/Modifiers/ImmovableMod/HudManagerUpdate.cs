using HarmonyLib;
using TownOfUs.Modifiers;

namespace TownOfUs.Modifiers.ImmovableMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Prefix(HudManager __instance)
        {
            foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Immovable))
            {
                var immovable = (Immovable)modifier;
                var player = immovable.Player;
                if (player == null || player.Data == null || !player.CanMove ||
                    player.Data.IsDead || player.Data.Disconnected) continue;
                immovable.Location = player.transform.localPosition;
            }
        }
    }
}
