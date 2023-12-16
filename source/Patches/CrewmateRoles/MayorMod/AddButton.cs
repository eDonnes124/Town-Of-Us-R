using System;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.MayorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddRevealButton
    {
        public static Sprite RevealSprite => TownOfUs.RevealSprite;

        public static void GenButton(Mayor role, int index)
        {
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = RevealSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Reveal(role));
            role.RevealButton = newButton;
        }


        private static Action Reveal(Mayor role)
        {
            void Listener()
            {
                role.RevealButton.Destroy();
                role.Revealed = true;
                Utils.Rpc(CustomRPC.Reveal, role.Player.PlayerId);
            }

            return Listener;
        }

        public static void RemoveAssassin(Mayor mayor)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == mayor.Player.PlayerId);
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin))
            {
                var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition += new Vector3(-0.2f, -0.1f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                var (cycleBack, cycleForward, guess, guessText) = doomsayer.Buttons[voteArea.TargetPlayerId];
                if (cycleBack == null || cycleForward == null) return;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                doomsayer.Buttons[voteArea.TargetPlayerId] = (null, null, null, null);
                doomsayer.Guesses.Remove(voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition += new Vector3(-0.2f, -0.1f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                var vigilante = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                var (cycleBack, cycleForward, guess, guessText) = vigilante.Buttons[voteArea.TargetPlayerId];
                if (cycleBack == null || cycleForward == null) return;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                vigilante.Buttons[voteArea.TargetPlayerId] = (null, null, null, null);
                vigilante.Guesses.Remove(voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition += new Vector3(-0.2f, -0.1f, 0f);
            }
            return;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Mayor))
            {
                var mayor = (Mayor)role;
                mayor.RevealButton.Destroy();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
            var mayorrole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
            if (mayorrole.Revealed) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (PlayerControl.LocalPlayer.PlayerId == __instance.playerStates[i].TargetPlayerId)
                {
                    GenButton(mayorrole, i);
                }
        }
    }
}