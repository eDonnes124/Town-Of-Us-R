using System.Linq;
using HarmonyLib;
using TownOfUs.Roles.Modifiers;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Modifiers.RadarMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrow
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var modifier in Modifier.AllModifiers)
            {
                if (x.ModifierType != ModifierEnum.Radar) continue;
                
                var radar = (Radar)modifier;
                if (radar.Player.Data.IsDead)
                {
                    radar.RadarArrow.DestroyAll();
                    radar.RadarArrow.Clear();
                }

                foreach (var arrow in radar.RadarArrow)
                {
                    radar.ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer, PlayerControl.AllPlayerControls);
                    arrow.target = radar.ClosestPlayer.transform.position;
                }
            }
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }
    }
}
