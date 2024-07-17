using HarmonyLib;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using AmongUs.GameOptions;
using TownOfUs.Roles;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdate
    {
        private static GameObject ZoomButton;
        private static GameObject MapColorButton;
        public static bool Zooming;
        public static bool Colorful = false;
        private static Vector3 Pos;
        private static Vector3 Pos2;

        public static void Postfix(HudManager __instance)
        {
            if (!ZoomButton)
            {
                ZoomButton = Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ZoomButton.GetComponent<PassiveButton>().OnClick = new();
                ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Zoom));
            }

            if (!MapColorButton)
            {
                MapColorButton = Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                MapColorButton.GetComponent<PassiveButton>().OnClick = new();
                MapColorButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(ColorfulMap));
            }

            Pos = __instance.MapButton.transform.localPosition + new Vector3(0.02f, -0.66f, 0f);
            Pos2 = Pos + new Vector3(0f, -0.66f, 0f);
            var dead = false;
            if (Utils.ShowDeadBodies)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    var haunter = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (haunter.Caught) dead = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (phantom.Caught) dead = true;
                }
                else dead = true;
            }
            ZoomButton.SetActive(!MeetingHud.Instance && dead && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal);
            ZoomButton.transform.localPosition = Pos2;
            ZoomButton.GetComponent<SpriteRenderer>().sprite = Zooming ? TownOfUs.ZoomPlusButton : TownOfUs.ZoomMinusButton;
            MapColorButton.SetActive(!MeetingHud.Instance && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal);
            MapColorButton.transform.localPosition = Pos;
            MapColorButton.GetComponent<SpriteRenderer>().sprite = Colorful ? TownOfUs.ColoressButton : TownOfUs.ColorfulButton;
        }

        public static void Zoom()
        {
            Zooming = !Zooming;
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }

        public static void ColorfulMap()
        {
            Colorful = Colorful ? false : true;
        }

        public static void ZoomStart()
        {
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }
    }
}