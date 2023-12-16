using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static Sprite LighterSprite => TownOfUs.LighterSprite;
        public static Sprite DarkerSprite => TownOfUs.DarkerSprite;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (player == null || player.Data.Disconnected) {
                return true;
            } else {
                return false;
            }
        }

        public static void GenButton(Medic role, PlayerVoteArea voteArea)
        {
            
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                return;
            }
            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == voteArea.TargetPlayerId);

            if (role.LightDarkColors[playerControl.GetDefaultOutfit().ColorId] == "lighter") {
                renderer.sprite = LighterSprite;
            } else {
                renderer.sprite = DarkerSprite;
            }
            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            var newButtonClickEvent = new Button.ButtonClickedEvent();
            newButtonClickEvent.AddListener(LighterDarkerHandler());
            newButton.GetComponent<PassiveButton>().OnClick = newButtonClickEvent;
            role.Buttons.Add(newButton);
        }

        private static Action LighterDarkerHandler()
        {
            //Used to avoid the Lighter/Darker Indicators causing any button problems by giving them their own Listener events.
            void Listener()
            {
            }

            return Listener;
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
            foreach (var voteArea in __instance.playerStates)
            {
                try {
                    GenButton(medicrole, voteArea);
                } catch {
                }
            }
        }
    }
}