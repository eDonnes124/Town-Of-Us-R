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
        public static bool Zooming;
        private static Vector3 Pos;

        private static GameObject SettingsButton;
        public static bool SettingsActive;
        private static Vector3 Pos2;

        public static void Postfix(HudManager __instance)
        {
            if (!SettingsButton)
            {
                SettingsButton = Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                SettingsButton.GetComponent<SpriteRenderer>().sprite = TownOfUs.SettingsButton;
                SettingsButton.GetComponent<PassiveButton>().OnClick = new();
                SettingsButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => OpenSettings(__instance)));
            }

            Pos = __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, 0f);
            SettingsButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal);
            SettingsButton.transform.localPosition = Pos;

            if (!ZoomButton)
            {
                ZoomButton = Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ZoomButton.GetComponent<PassiveButton>().OnClick = new();
                ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Zoom));
            }

            Pos2 = Pos + new Vector3(0, -0.66f, 0f);
            var dead = (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) && !PlayerControl.LocalPlayer.Is(RoleEnum.Phantom)) || (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) &&
                Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught);
            ZoomButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && dead &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal && PlayerControl.LocalPlayer.Data.IsDead);
            ZoomButton.transform.localPosition = Pos2;
            ZoomButton.GetComponent<SpriteRenderer>().sprite = Zooming ? TownOfUs.ZoomPlusButton : TownOfUs.ZoomMinusButton;
            __instance.GameSettings.text = GameSettings.Settings();
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

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
        }

        public static void OpenSettings(HudManager __instance)
        {
            SettingsActive = !SettingsActive;
            __instance.GameSettings.gameObject.SetActive(SettingsActive);
        }
    }
}