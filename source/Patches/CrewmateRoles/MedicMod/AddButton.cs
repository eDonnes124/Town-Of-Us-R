using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static Sprite LighterSprite => TownOfUs.LighterSprite;
        public static Sprite DarkerSprite => TownOfUs.DarkerSprite;

        public static void GenButton(Medic role, int index)
        {
            var colorButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == MeetingHud.Instance.playerStates[index].TargetPlayerId);

            if (role.LightDarkColors[playerControl.CurrentOutfit.ColorId] == "lighter") {
                renderer.sprite = LighterSprite;
            } else {
                renderer.sprite = DarkerSprite;
            }
            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            role.Buttons.Add(newButton);
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Medic))
            {
                var medic = (Medic) role;
                medic.Buttons.Clear();
            }
            if (CustomGameOptions.MedicReportColorDuration == 0) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var medicrole = Role.GetRole<Medic>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
                GenButton(medicrole, i);
        }
    }
}