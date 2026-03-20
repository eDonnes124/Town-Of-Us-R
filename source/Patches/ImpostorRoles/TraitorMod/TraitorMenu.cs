using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.TraitorMod
{
    public class TraitorMenu
    {
        public ShapeshifterMinigame Menu;
        public Select Click;
        public List<RoleEnum> Targets;
        public static TraitorMenu singleton;
        public delegate void Select(RoleEnum roleType);

        public TraitorMenu(Select click)
        {
            Click = click;
            if (singleton != null)
            {
                singleton.Menu.DestroyImmediate();
                singleton = null;
            }
            singleton = this;
        }

        public IEnumerator Open(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            while (ExileController.Instance != null) { yield return 0; }
            var traitor = Role.GetRole<Traitor>(PlayerControl.LocalPlayer);
            Targets = traitor.CanBeRoles;
            Reactor.Utilities.Logger<TownOfUs>.Warning($"Targets {Targets.Count}");
            if (Menu == null)
            {
                if (Camera.main == null)
                    yield break;

                Menu = GameObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            }

            Menu.transform.SetParent(Camera.main.transform, false);
            Menu.transform.localPosition = new(0f, 0f, -50f);
            Menu.Begin(null);
        }

        private static ShapeshifterMinigame GetShapeshifterMenu()
        {
            var rolePrefab = RoleManager.Instance.AllRoles.ToArray().First(r => r.Role == RoleTypes.Shapeshifter);
            return GameObject.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
        }

        public void Clicked(RoleEnum roleType)
        {
            Click(roleType);
            Menu.Close();
        }


        [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
        public static class MenuPatch
        {
            public static bool Prefix(ShapeshifterMinigame __instance)
            {
                var menu = TraitorMenu.singleton;

                if (menu == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Traitor))
                    return true;

                __instance.potentialVictims = new();
                var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

                for (var i = 0; i < menu.Targets.Count; i++)
                {
                    var roleType = menu.Targets[i];
                    string name = PlayerControl.LocalPlayer.Data.PlayerName;
                    PlayerControl.LocalPlayer.Data.PlayerName = roleType.ToString();
                    var num = i % 3;
                    var num2 = i / 3;
                    var panel = GameObject.Instantiate(__instance.PanelPrefab, __instance.transform);
                    panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                    panel.SetPlayer(i, PlayerControl.LocalPlayer.Data, (Action)(() => menu.Clicked(roleType)));
                    __instance.potentialVictims.Add(panel);
                    list2.Add(panel.Button);
                    PlayerControl.LocalPlayer.Data.PlayerName = name;
                }

                ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
                return false;
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public static class StartMeeting
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (__instance == null) return;
                try
                {
                    TraitorMenu.singleton.Menu.Close();
                }
                catch { }
            }
        }
    }
}
