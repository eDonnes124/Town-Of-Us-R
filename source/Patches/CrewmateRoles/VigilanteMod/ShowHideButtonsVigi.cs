using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine.UI;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.VigilanteMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public class ShowHideButtonsVigi
    {
        public static void HideButtonsVigi(Vigilante role)
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
            Vigilante role,
            byte targetId,
            bool killedSelf
        )
        {
            if (
                killedSelf ||
                role.RemainingKills == 0 ||
                (!CustomGameOptions.VigilanteMultiKill)
            ) HideButtonsVigi(role);
            else HideTarget(role, targetId);
        }
        public static void HideTarget(
            Vigilante role,
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
