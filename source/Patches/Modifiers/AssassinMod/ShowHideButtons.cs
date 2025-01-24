using HarmonyLib;
using System.Linq;
using TownOfUs.Roles.Modifiers;
using UnityEngine.UI;
using UnityEngine;

namespace TownOfUs.Modifiers.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public class ShowHideButtons
    {
        public static void HideButtons(Assassin role)
        {
            foreach (var (_, (cycleBack, cycleForward, guess, guessText)) in role.Buttons)
            {
                if (cycleBack == null || cycleForward == null) continue;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                role.GuessedThisMeeting = true;
            }

            foreach (var voteArea in MeetingHud.Instance.playerStates)
            {
                voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
            }
        }

        public static void HideSingle(
            Assassin role,
            byte targetId,
            bool killedSelf,
            bool doubleshot = false
        )
        {
            if (
                (killedSelf ||
                role.RemainingKills == 0 ||
                (!CustomGameOptions.AssassinMultiKill))
                && doubleshot == false
            ) HideButtons(role);
            else HideTarget(role, targetId);
        }
        public static void HideTarget(
            Assassin role,
            byte targetId
        )
        {
            var (cycleBack, cycleForward, guess, guessText) = role.Buttons[targetId];
            if (cycleBack == null || cycleForward == null) return;
            cycleBack.SetActive(false);
            cycleForward.SetActive(false);
            guess.SetActive(false);
            guessText.gameObject.SetActive(false);

            cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            role.Buttons[targetId] = (null, null, null, null);
            role.Guesses.Remove(targetId);

            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == targetId);
            voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
        }
    }
}
