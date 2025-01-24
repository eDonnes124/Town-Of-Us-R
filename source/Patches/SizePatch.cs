using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;
using System;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public static class SizePatch
    {
        public static float Radius = 0.2233912f;
        public static float Offset = 0.3636057f;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                CircleCollider2D collider = player.Collider.Caster<CircleCollider2D>();
                if (player.Data != null && !(player.Data.IsDead || player.Data.Disconnected))
                {
                    player.transform.localScale = player.GetAppearance().SizeFactor;
                    if (player.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                    {
                        collider.radius = Radius * 1.75f;
                        collider.offset = Offset / 1.75f * Vector2.down;
                    }
                    else
                    {
                        collider.radius = Radius;
                        collider.offset = Offset * Vector2.down;
                    }
                }
                else
                {
                    player.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
                    collider.radius = Radius;
                    collider.offset = Offset * Vector2.down;
                }
            }

            var playerBindings = PlayerControl.AllPlayerControls.ToArray().ToDictionary(player => player.PlayerId);
            var bodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            foreach (var body in bodies)
            {
                try {
                    body.transform.localScale = playerBindings[body.ParentId].GetAppearance().SizeFactor;
                } catch {
                }
            }
        }
    }
}