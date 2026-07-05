using System.Collections.Generic;
using System.Reflection;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs.CustomOption;
using TownOfUs.Utilities;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal static class DisplayRoleList
    {
        public static GameObject RoleList;
        public static TextMeshPro RoleListTextComp;

        private static readonly List<string> roleListText = new List<string>
        {
            "<color=#66FFFFFF>Crew</color> Investigative",
            "<color=#66FFFFFF>Crew</color> Killing",
            "<color=#66FFFFFF>Crew</color> Power",
            "<color=#66FFFFFF>Crew</color> Protective",
            "<color=#66FFFFFF>Crew</color> Support",
            "Common <color=#66FFFFFF>Crew</color>",
            "Special <color=#66FFFFFF>Crew</color>",
            "Random <color=#66FFFFFF>Crew</color>",
            "<color=#999999FF>Neutral</color> Benign",
            "<color=#999999FF>Neutral</color> Evil",
            "<color=#999999FF>Neutral</color> Killing",
            "Common <color=#999999FF>Neutral</color>",
            "Random <color=#999999FF>Neutral</color>",
            "<color=#FF0000FF>Imp</color> Concealing",
            "<color=#FF0000FF>Imp</color> Killing",
            "<color=#FF0000FF>Imp</color> Support",
            "Common <color=#FF0000FF>Imp</color>",
            "Random <color=#FF0000FF>Imp</color>",
            "Non-<color=#FF0000FF>Imp</color>",
            "Any"
        };

        private static string GetRoleForSlot(int slotValue)
        {
            if (slotValue >= 0 && slotValue < roleListText.Count)
            {
                return roleListText[slotValue]; 
            }
            else
            {
                return "<color=#696969>Unknown</color>"; 
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

            if (!LobbyBehaviour.Instance)
            {
                if (RoleList)
                {
                    RoleList.SetActive(false);
                }

                return;
            }

            if (!RoleList)
            {
                var pingTracker = Object.FindObjectOfType<PingTracker>(true);
                RoleList = Object.Instantiate(pingTracker.gameObject, __instance.transform);
                RoleList.name = "RoleListText";
                var pos = RoleList.gameObject.GetComponent<AspectPosition>();
                pos.Alignment = AspectPosition.EdgeAlignments.LeftTop;
                pos.DistanceFromEdge = new Vector3(0.43f, 0.1f, 1f);
                RoleList.GetComponent<PingTracker>().Destroy();

                RoleListTextComp = RoleList.GetComponent<TextMeshPro>();
                RoleListTextComp.alignment = TextAlignmentOptions.TopLeft;
                RoleListTextComp.verticalAlignment = VerticalAlignmentOptions.Top;
                RoleListTextComp.fontSize = RoleListTextComp.fontSizeMin = RoleListTextComp.fontSizeMax = 3f;
                RoleList.SetActive(false);
            }
            
            if (RoleList)
            {
                string rolelist = string.Empty;

                var players = GameData.Instance.PlayerCount;
                int maxSlots = players < 15 ? players : 15; 

                rolelist = string.Empty;
                for (int i = 0; i < maxSlots; i++)
                {
                    int slotValue = i switch
                    {
                        0 => Generate.Slot1.Get(), 
                        1 => Generate.Slot2.Get(), 
                        2 => Generate.Slot3.Get(), 
                        3 => Generate.Slot4.Get(), 
                        4 => Generate.Slot5.Get(), 
                        5 => Generate.Slot6.Get(), 
                        6 => Generate.Slot7.Get(), 
                        7 => Generate.Slot8.Get(), 
                        8 => Generate.Slot9.Get(), 
                        9 => Generate.Slot10.Get(), 
                        10 => Generate.Slot11.Get(), 
                        11 => Generate.Slot12.Get(), 
                        12 => Generate.Slot13.Get(), 
                        13 => Generate.Slot14.Get(),
                        14 => Generate.Slot15.Get(),
                        _ => -1 
                    };

                    rolelist += $"{GetRoleForSlot(slotValue)}\n";  
                }
                RoleListTextComp.text = $"<color=#FFD700>Role List:</color>\n{rolelist}";
                RoleList.SetActive(true);
            }
        }
    }
}
