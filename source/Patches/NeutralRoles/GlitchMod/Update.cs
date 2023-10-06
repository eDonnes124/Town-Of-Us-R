using System.Linq;
using HarmonyLib;
using InnerNet;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal class Update
    {
        private static void Postfix(HudManager __instance)
        {
            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (glitch != null)
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                        Role.GetRole<Glitch>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.UpdateChatMode))]
    class chatModeUpdate
    {
        private static bool Prefix(ChatController __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) return true;
            return (__instance != Role.GetRole<Glitch>(PlayerControl.LocalPlayer).MimicList);
        }
    }
}