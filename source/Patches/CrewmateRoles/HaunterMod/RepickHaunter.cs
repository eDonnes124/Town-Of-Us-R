using HarmonyLib;
using System.Linq;
using Hazel;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickHaunter
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetHaunter.WillBeHaunter) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetHaunter.WillBeHaunter = pc;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
            writer.Write(pc.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return;
        }
    }
}