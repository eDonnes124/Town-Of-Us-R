using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class TempJail
    {
        public static Sprite CellSprite => TownOfUs.InJailSprite;

        public static void GenCell(Imitator role, PlayerVoteArea voteArea)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;

            var jailCell = Object.Instantiate(confirmButton, voteArea.transform);
            var cellRenderer = jailCell.GetComponent<SpriteRenderer>();
            var passive = jailCell.GetComponent<PassiveButton>();
            cellRenderer.sprite = CellSprite;
            jailCell.transform.localPosition = new Vector3(-0.95f, 0f, -2f);
            jailCell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            jailCell.layer = 5;
            jailCell.transform.parent = parent;
            jailCell.transform.GetChild(0).gameObject.Destroy();

            passive.OnClick = new Button.ButtonClickedEvent();
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Imitator))
            {
                var imitator = (Imitator)role;
                if (imitator.jailedPlayer == null) return;
                if (imitator.Player.Data.IsDead || imitator.Player.Data.Disconnected) return;
                if (imitator.jailedPlayer.Data.IsDead || imitator.jailedPlayer.Data.Disconnected) return;
                foreach (var voteArea in __instance.playerStates)
                    if (imitator.jailedPlayer.PlayerId == voteArea.TargetPlayerId)
                    {
                        GenCell(imitator, voteArea);
                    }
            }
        }
    }
}