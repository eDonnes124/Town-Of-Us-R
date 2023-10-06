using HarmonyLib;
using InnerNet;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TransporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Transporter)) return;
            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (role != null)
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                        Role.GetRole<Transporter>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.UpdateChatMode))]
    class chatModeUpdate
    {
        private static bool Prefix(ChatController __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Transporter)) return true;
            return (__instance != Role.GetRole<Transporter>(PlayerControl.LocalPlayer).TransportList);
        }
    }
}