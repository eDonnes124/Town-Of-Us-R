using HarmonyLib;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Extensions;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHud_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            Utils.ShowDeadBodies = PlayerControl.LocalPlayer.Data.IsDead;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class MeetingHud_Close
    {
        public static void Postfix(MeetingHud __instance)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.RemoveAllBodies, SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            var buggedBodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in buggedBodies)
            {
                body.gameObject.Destroy();
            }
        }
    }
}