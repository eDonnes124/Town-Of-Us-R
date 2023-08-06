using HarmonyLib;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Modifiers.DisperserMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return true;

            var role = Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer);
            if (__instance != role.RoleAbilityButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.ButtonUsed) return false;
            if (role.StartTimer() > 0) return false;
            if (!__instance.enabled) return false;

            role.ButtonUsed = true;

            role.Disperse();

            return false;
        }
    }
}