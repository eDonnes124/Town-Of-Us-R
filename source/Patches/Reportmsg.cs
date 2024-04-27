using HarmonyLib;
using Hazel;

namespace TownOfUs
{
    public class Reportmessage
    {
        public static string location;
        
        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public class Recordlocation
        {
            [HarmonyPostfix]
            public static void postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.localPosition.y != -3.25f)
                {
                    location = __instance.text.text;
                }
                else
                {
                    string name = PlayerControl.LocalPlayer.name;
                    location = $"a hallway/outside, {name} where is the body?";
                }
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        public class Sendchat
        {
            [HarmonyPostfix]
            public static void postfix([HarmonyArgument(0)] GameData.PlayerInfo target)
            {
                string report = $"Body reported in: {location}";
                if (target != null && CustomGameOptions.Locationreports)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                    Utils.Rpc(CustomRPC.SendChat, report);
                }
                
            }
        }
    }
}
