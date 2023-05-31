using HarmonyLib;
using System.Linq;
using Hazel;
using UnityEngine;

namespace TownOfUs.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickPhantom
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetPhantom.WillBePhantom) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Crewmates) && !x.Is(Faction.Impostors) && !x.Is(ModifierEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (!PlayerControl.LocalPlayer.Is(Faction.NeutralKilling) && !PlayerControl.LocalPlayer.Is(Faction.NeutralOther))
            {
                var toChooseFromAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralOther)) && !x.Is(ModifierEnum.Lover) && !x.Data.Disconnected).ToList();
                if (toChooseFromAlive.Count == 0)
                {
                    SetPhantom.WillBePhantom = null;

                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                    writer2.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                }
                else
                {
                    var rand2 = Random.RandomRangeInt(0, toChooseFromAlive.Count);
                    var pc2 = toChooseFromAlive[rand2];

                    SetPhantom.WillBePhantom = pc2;

                    var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                    writer3.Write(pc2.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer3);
                }
                return;
            }
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetPhantom.WillBePhantom = pc;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
            writer.Write(pc.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return;
        }
    }
}