using System.Collections.Generic;
using HarmonyLib;
using TMPro;
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

        public static void UpdateBlips(CounterArea area, List<int> colorMapping)
        {
            area.UpdateCount(colorMapping.Count);
            var icons = area.myIcons.ToArray();
            colorMapping.Sort();
            var useCompactText = icons.Count > 2 * area.MaxWidth;
            for (var i = 0;i < colorMapping.Count;i++)
            {
                var icon = icons[i];
                var sprite = icon.GetComponent<SpriteRenderer>();
                var text = icon.GetComponentInChildren<TextMeshPro>(true);
                if (sprite != null)
                {
                    PlayerControl.SetPlayerMaterialColors(colorMapping[i], sprite);
                }
                if (text != null) {
                    text.gameObject.SetActive(true);
                    text.text = colorMapping[i].ToString();
                    // Show first row numbers below player icons
                    // Show second row numbers above player icons
                    // show all icons on player icons when there are three rows
                    if(useCompactText) {
                        text.transform.localPosition = new Vector3(0, 0, -20);
					} else if (i / area.MaxWidth == 0) {
                        text.transform.localPosition = new Vector3(0, -area.YOffset, -20);
                    } else {
                        text.transform.localPosition = new Vector3(0, area.YOffset, -20);
                    }
                }
            }
        }

        public static void UpdateBlips(MapCountOverlay __instance)
        {
            var rooms = ShipStatus.Instance.FastRooms;
            foreach (var area in __instance.CountAreas)
            {
                if (!rooms.ContainsKey(area.RoomType)) continue;
                var room = rooms[area.RoomType];
                if (room.roomArea == null) continue;
                var objectsInRoom = room.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                var colorMap = new List<int>();
                for (var i = 0;i < objectsInRoom;i++)
                {
                    var collider = __instance.buffer[i];
                    if (collider.tag == "DeadBody")
                    {
                        var playerId = collider.GetComponent<DeadBody>().ParentId;
                        colorMap.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                        continue;
                    }
                    var player = collider.GetComponent<PlayerControl>();
                    var data = player?.Data;
                    if (data != null && !data.Disconnected && !data.IsDead)
                        colorMap.Add(data.DefaultOutfit.ColorId);
                }
                UpdateBlips(area, colorMap);
            }
        }

        public static bool Prefix(MapCountOverlay __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (!localPlayer.Is(RoleEnum.Spy)) return true;
            __instance.timer += Time.deltaTime;
            if (__instance.timer < 0.1f) return false;

            __instance.timer = 0f;

            var sabotaged = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(localPlayer);

            if (sabotaged != __instance.isSab)
                SetSabotaged(__instance, sabotaged);

            if (!sabotaged)
                UpdateBlips(__instance);
            return false;
        }
    }
}