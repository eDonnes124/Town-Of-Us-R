using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;
using Hazel;
using TownOfUs.Extensions;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            if (target.IsOnAlert())
            {
                if (__instance.isActiveAndEnabled && !__instance.isCoolingDown)
                {
                    if (target.IsShielded())
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks) PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                        else PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                        StopKill.BreakShield(medic, target.PlayerId,
                            CustomGameOptions.ShieldBreaks);
                        Utils.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
                    }
                    else if (PlayerControl.LocalPlayer.IsShielded())
                    {
                        var medic = PlayerControl.LocalPlayer.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks) PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                        else PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                        StopKill.BreakShield(medic, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                        if (CustomGameOptions.KilledOnAlert)
                        {
                            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                        }
                    }
                    else
                    {
                        Utils.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
                        if (CustomGameOptions.KilledOnAlert)
                        {
                            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                        }
                    }
                }
                return false;
            }
            else if (target.IsShielded())
            {
                if (__instance.isActiveAndEnabled && !__instance.isCoolingDown)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(target.GetMedic().Player.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                    if (CustomGameOptions.ShieldBreaks) PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                    else PlayerControl.LocalPlayer.SetKillTimer(0.01f);
                    StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                return false;
            }
            return true;
        }
    }
}