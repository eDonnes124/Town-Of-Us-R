using HarmonyLib;
using Hazel;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public class MurderPlayer
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class MurderPlayerPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                Utils.MurderPlayer(__instance, target);
                return false;
            }
        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        [HarmonyPriority(Priority.Last)]
        public class DoClickPatch
        {
            public static bool Prefix(KillButton __instance)
            {
                if (__instance.isActiveAndEnabled && __instance.currentTarget && !__instance.isCoolingDown && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove)
                {
                    if (AmongUsClient.Instance.AmHost)
                    {
                        PlayerControl.LocalPlayer.CheckMurder(__instance.currentTarget);
                    }
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.CheckMurder, SendOption.Reliable, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(__instance.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    __instance.SetTarget(null);
                }
                return false;
            }
        }
    }
}