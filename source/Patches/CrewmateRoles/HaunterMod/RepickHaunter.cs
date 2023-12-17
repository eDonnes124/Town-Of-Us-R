using HarmonyLib;
using System.Linq;
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
            if (!PlayerControl.LocalPlayer.Is(Faction.Crewmates))
            {
                var toChooseFromAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && !x.Data.Disconnected).ToList();
                if (toChooseFromAlive.Count == 0)
                {
                    SetHaunter.WillBeHaunter = null;

                    Utils.Rpc(CustomRPC.SetHaunter, byte.MaxValue);
                }
                else
                {
                    var rand2 = Random.RandomRangeInt(0, toChooseFromAlive.Count);
                    var pc2 = toChooseFromAlive[rand2];

                    SetHaunter.WillBeHaunter = pc2;

                    Utils.Rpc(CustomRPC.SetHaunter, pc2.PlayerId);
                }
                return;
            }
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetHaunter.WillBeHaunter = pc;

            Utils.Rpc(CustomRPC.SetHaunter, pc.PlayerId);
            return;
        }
    }
}