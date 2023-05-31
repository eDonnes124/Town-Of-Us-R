using System.Collections.Generic;
using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    public static class Admin
    {
        public static void SetSabotaged(MapCountOverlay __instance, bool sabotaged)
        {
            __instance.isSab = sabotaged;
            __instance.BackgroundColor.SetColor(sabotaged ? Palette.DisabledGrey : Color.green);
            __instance.SabotageText.gameObject.SetActive(sabotaged);
            if (sabotaged)
                foreach (var area in __instance.CountAreas)
                    area.UpdateCount(0);
        }

        public static void UpdateBlips(CounterArea area, List<int> colorMapping, bool isSpy)
        {
            area.UpdateCount(colorMapping.Count);
            var icons = area.myIcons.ToArray();
            colorMapping.Sort();
            for (var i = 0;i < colorMapping.Count;i++)
            {
                var icon = icons[i];
                var sprite = icon.GetComponent<SpriteRenderer>();
                if (Patches.SubmergedCompatibility.Loaded) sprite.color = new Color(1, 1, 1, 1);
                if (sprite != null)
                {
                    if (isSpy) PlayerMaterial.SetColors(colorMapping[i], sprite);
                    else PlayerMaterial.SetColors(new Color(0.8793f, 1, 0, 1), sprite);
                }
            }
        }

        public static void UpdateBlips(MapCountOverlay __instance, bool isSpy)
        {
            var rooms = ShipStatus.Instance.FastRooms;
            var colorMapDuplicate = new List<int>();
            foreach (var area in __instance.CountAreas)
            {
                if (!rooms.ContainsKey(area.RoomType)) continue;
                var room = rooms[area.RoomType];
                if (room.roomArea == null) continue;
                var objectsInRoom = room.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                var colorMap = new List<int>();
                for (var i = 0; i < objectsInRoom; i++)
                {
                    var collider = __instance.buffer[i];
                    var player = collider.GetComponent<PlayerControl>();
                    var data = player?.Data;
                    if (collider.tag == "DeadBody" &&
                        (isSpy && CustomGameOptions.WhoSeesDead == AdminDeadPlayers.Spy ||
                        !isSpy && CustomGameOptions.WhoSeesDead == AdminDeadPlayers.EveryoneButSpy ||
                        CustomGameOptions.WhoSeesDead == AdminDeadPlayers.Everyone))
                    {
                        var playerId = collider.GetComponent<DeadBody>().ParentId;
                        colorMap.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                        colorMapDuplicate.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                        continue;
                    }
                    else
                    {
                        PlayerControl component = collider.GetComponent<PlayerControl>();
                        if (component && component.Data != null && !component.Data.Disconnected && !component.Data.IsDead && (__instance.showLivePlayerPosition || !component.AmOwner))
                        {
                            if (!colorMapDuplicate.Contains(data.DefaultOutfit.ColorId))
                            {
                                colorMap.Add(data.DefaultOutfit.ColorId);
                                colorMapDuplicate.Add(data.DefaultOutfit.ColorId);
                            }
                        }
                    }
                }
                UpdateBlips(area, colorMap, isSpy);
            }
        }

        public static bool Prefix(MapCountOverlay __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
            var localPlayer = PlayerControl.LocalPlayer;
            var isSpy = localPlayer.Is(RoleEnum.Spy);
            __instance.timer += Time.deltaTime;
            if (__instance.timer < 0.1f) return false;

            __instance.timer = 0f;

            var sabotaged = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(localPlayer);

            if (sabotaged != __instance.isSab)
                SetSabotaged(__instance, sabotaged);

            if (!sabotaged)
                UpdateBlips(__instance, isSpy);
            return false;
        }
    }
}