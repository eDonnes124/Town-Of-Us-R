using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.PlumberMod
{
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    public class PerformVent
    {
        public static Sprite Arrow => TownOfUs.Arrow;
        public static bool Prefix(VentButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Plumber);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled || __instance.isCoolingDown) return false;
            var role = Role.GetRole<Plumber>(PlayerControl.LocalPlayer);
            if (role.FlushTimer() > 0f || __instance.currentTarget == null) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;

            PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"{role.Vent.Id}");
            var someoneInVent = false;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.inVent) someoneInVent = true;
            }
            if (someoneInVent)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Crewmate));
                Coroutines.Start(SeeVenter());
            }
            role.LastFlushed = DateTime.UtcNow;
            Utils.Rpc(CustomRPC.Flush, (byte)0);
            return false;
        }

        public static IEnumerator SeeVenter()
        {
            var startTime = DateTime.UtcNow;
            var arrows = new Dictionary<byte, ArrowBehaviour>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.inVent)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = player.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrows.Add(player.PlayerId, arrow);
                }
            }

            while ((DateTime.UtcNow - startTime).TotalSeconds < 1f)
            {
                foreach (var arrow in arrows)
                {
                    arrows.GetValueSafe(arrow.Key).target = Utils.PlayerById(arrow.Key).transform.localPosition;
                }
                yield return null;
            }

            arrows.Values.DestroyAll();
            arrows.Clear();
        }
    }
}