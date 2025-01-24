using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButtonImitator
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfUs.ImitateSelectSprite;
        public static Sprite DisabledSprite => TownOfUs.ImitateDeselectSprite;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (
                    player == null ||
                    !player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            return !player.Is(Faction.Crewmates);
        }


        public static void GenButton(Imitator role, PlayerVoteArea voteArea, bool replace = false)
        {
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                if (replace) return;
                role.Buttons.Add(null);
                role.ListOfActives.Add((targetId, false));
                return;
            }

            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, targetId));
            if (replace)
            {
                for (var i = 0; i < role.Buttons.Count; i++)
                {
                    if (role.ListOfActives[i].Item1 == targetId)
                    {
                        role.Buttons[i] = newButton;
                    }
                }
            }
            else
            {
                role.Buttons.Add(newButton);
                role.ListOfActives.Add((targetId, false));
            }
        }


        private static Action SetActive(Imitator role, int targetId)
        {
            void Listener()
            {
                int index = int.MaxValue;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (role.ListOfActives[i].Item1 == targetId)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == int.MaxValue) return;
                if (role.ListOfActives.Count(x => x.Item2) == 1 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                {
                    int active = 0;
                    for (var i = 0; i < role.ListOfActives.Count; i++) if (role.ListOfActives[i].Item2) active = i;

                    role.Buttons[active].GetComponent<SpriteRenderer>().sprite =
                        role.ListOfActives[active].Item2 ? DisabledSprite : ActiveSprite;

                    role.ListOfActives[active] = (role.ListOfActives[active].Item1, !role.ListOfActives[active].Item2);
                }

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index].Item2 ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = (role.ListOfActives[index].Item1, !role.ListOfActives[index].Item2);

                _mostRecentId = index;

                SetImitate.Imitate = null;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i].Item2) continue;
                    SetImitate.Imitate = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Imitator))
            {
                var imitator = (Imitator)role;
                imitator.ListOfActives.Clear();
                imitator.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var imitatorRole = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
            foreach (var voteArea in __instance.playerStates)
            {
                GenButton(imitatorRole, voteArea);
            }
        }
    }
}