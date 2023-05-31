using HarmonyLib;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.JesterButton)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            if (!CustomGameOptions.ExecutionerButton)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            if (!CustomGameOptions.SwapperButton)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public class NoButtonsHost
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.JesterButton) 
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            if (!CustomGameOptions.ExecutionerButton)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            if (!CustomGameOptions.SwapperButton)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }
}