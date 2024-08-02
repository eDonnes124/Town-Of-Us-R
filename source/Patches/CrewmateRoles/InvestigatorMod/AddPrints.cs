using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class AddPrints
    {
        private static float _time;

        public static bool GameStarted = false;
        private static float Interval => CustomGameOptions.FootprintInterval;
        private static bool Vent => CustomGameOptions.VentFootprintVisible;

        private static Vector2 Position(PlayerControl player)
        {
            return player.GetTruePosition() + new Vector2(0, 0.366667f);
        }


        public static void Postfix(HudManager __instance)
        {
            if (!GameStarted || !PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;
            // New Footprint
            var investigator = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
            _time += Time.deltaTime;
            if (_time >= Interval)
            {
                _time -= Interval;
                    if (!(investigator.InvestigatedPlayer == null || investigator.InvestigatedPlayer.Data.IsDead ||
                        investigator.InvestigatedPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId))
                    {
                    var canPlace = !investigator.AllPrints.Any(print =>
                        Vector3.Distance(print.Position, Position(investigator.InvestigatedPlayer)) < 0.5f &&
                        print.Color.a > 0.5 &&
                        print.Player.PlayerId == investigator.InvestigatedPlayer.PlayerId);

                    if (Vent && ShipStatus.Instance != null)
                        if (ShipStatus.Instance.AllVents.Any(vent =>
                            Vector2.Distance(vent.gameObject.transform.position, Position(investigator.InvestigatedPlayer)) < 1f))
                            canPlace = false;

                    if (canPlace) new Footprint(investigator.InvestigatedPlayer, investigator);
                    }

                for (var i = 0; i < investigator.AllPrints.Count; i++)
                {
                    try
                    {
                        var footprint = investigator.AllPrints[i];
                        if (footprint.Update()) i--;
                    } catch
                    {
                        //assume footprint value is null and allow the loop to continue
                        continue;
                    }
                    
                }
            }
        }
    }
}