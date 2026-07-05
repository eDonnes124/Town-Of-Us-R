using System.Linq;
using HarmonyLib;
using TownOfUs.Modifiers;
using System.Collections.Generic;
using UnityEngine;
using TownOfUs.Patches;

namespace TownOfUs.Modifiers.RadarMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateArrow
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Radar)) return;

            var radar = Modifier.GetModifier<Radar>(PlayerControl.LocalPlayer);
            if (radar.Player.Data.IsDead)
            {
                radar.RadarArrow.DestroyAll();
                radar.RadarArrow.Clear();
                return;
            }

            if (radar.RadarArrow.Count == 0)
            {
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                renderer.color = Colors.Radar;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = PlayerControl.LocalPlayer.transform.position;
                radar.RadarArrow.Add(arrow);
            }

            foreach (var arrow in radar.RadarArrow)
            {
                radar.ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer, PlayerControl.AllPlayerControls.ToArray().ToList());
                arrow.target = radar.ClosestPlayer.transform.position;
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