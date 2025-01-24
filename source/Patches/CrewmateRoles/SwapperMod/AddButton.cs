using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.SwapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfUs.SwapperSwitch;
        public static Sprite DisabledSprite => TownOfUs.SwapperSwitchDisabled;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (player.IsJailed()) return true;
            if (
                    player == null ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            return false;
        }

        public static void GenButton(Swapper role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
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
            role.Buttons.Add(newButton);
            role.ListOfActives.Add((targetId, false));
        }


        private static Action SetActive(Swapper role, int targetId)
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
                if (role.ListOfActives.Count(x => x.Item2) == 2 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite) return;

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index].Item2 ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = (role.ListOfActives[index].Item1, !role.ListOfActives[index].Item2);

                _mostRecentId = index;

                SwapVotes.Swap1 = null;
                SwapVotes.Swap2 = null;
                var toSet1 = true;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i].Item2) continue;

                    if (toSet1)
                    {
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates[i];
                        toSet1 = false;
                    }
                    else
                    {
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates[i];
                    }
                }

                if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null)
                {
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    return;
                }

                Utils.Rpc(CustomRPC.SetSwaps, SwapVotes.Swap1.TargetPlayerId, SwapVotes.Swap2.TargetPlayerId);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Swapper))
            {
                var swapper = (Swapper) role;
                swapper.ListOfActives.Clear();
                swapper.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swapper)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var swapperrole = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
            foreach (var voteArea in __instance.playerStates)
            {
                GenButton(swapperrole, voteArea);
            }
        }
    }
}