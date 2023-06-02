using System;
using HarmonyLib;

namespace TownOfUs.Modifiers.LoversMod
{
    public static class Chat
    {
        private static DateTime MeetingStartTime = DateTime.MinValue;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Prefix(MeetingHud __instance)
            {
                MeetingStartTime = DateTime.UtcNow;
            }
        }
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
            {
                if (__instance != HudManager.Instance.Chat) return true;
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;
                Boolean shouldSeeMessage = localPlayer.Data.IsDead || localPlayer.IsLover() ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                if (DateTime.UtcNow - MeetingStartTime < TimeSpan.FromSeconds(1))
                {
                    return shouldSeeMessage;
                }
                return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || shouldSeeMessage;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.IsLover() & !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
            }
        }
    }
}