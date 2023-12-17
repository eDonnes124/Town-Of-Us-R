using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.TraitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickTraitor
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetTraitor.WillBeTraitor) return;
            if (PlayerControl.LocalPlayer.Is(Faction.Impostors)) return;
            if (!PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return;
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor) &&
                !x.Is(ModifierEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected && !x.IsExeTarget()).ToList();
            if (toChooseFrom.Count == 0)
            {
                SetTraitor.WillBeTraitor = null;
                Utils.Rpc(CustomRPC.SetTraitor, byte.MaxValue);
            }
            else
            {
                var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                var pc = toChooseFrom[rand];

                SetTraitor.WillBeTraitor = pc;

                Utils.Rpc(CustomRPC.SetTraitor, pc.PlayerId);
            }
            return;
        }
    }
}
