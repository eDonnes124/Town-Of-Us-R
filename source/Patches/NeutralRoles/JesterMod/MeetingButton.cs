using System;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingButton
    {
        public static Sprite KillSprite => TownOfUs.KillSprite;

        public static void GenButton (Jester role, PlayerVoteArea voteArea)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;
            var newButton = Object.Instantiate(confirmButton, parent);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = KillSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.6f, 0.15f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            var buttonCollider = newButton.GetComponent<BoxCollider2D>();
            buttonCollider.size = renderer.sprite.bounds.size;
            buttonCollider.offset = Vector2.zero;

            newButton.transform.GetChild(0).gameObject.Destroy();

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Kill(role, voteArea));
            role.meetingButtons[voteArea.TargetPlayerId] = newButton;
        }

        private static Action Kill(Jester role, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                foreach (var button in role.meetingButtons) button.Value.Destroy();
                role.meetingButtons.Clear();

                JesterKill.RpcMurderPlayer(Role.GetRole(voteArea).Player, PlayerControl.LocalPlayer);
            }

            return Listener;
        }

                public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Jester))
            {
                var jesterRole = (Jester)role;
                foreach (var button in jesterRole.meetingButtons) button.Value.Destroy();
                jesterRole.meetingButtons.Clear();
            }

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) return;
            var jester = Role.GetRole<Jester>(PlayerControl.LocalPlayer);
            if (!jester.canKill) return;

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerState = __instance.playerStates[i];

                if (!jester.KillableVoters.Contains(playerState.TargetPlayerId) || playerState.AmDead) continue;

                GenButton(jester, playerState);
            }
        }
    }
}
