using HarmonyLib;
using TownOfUs.Roles.Modifiers;
using UnityEngine.UI;

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
        }

        public static void HideSingle(
            Assassin role,
            byte targetId,
            bool killedSelf
        )
        {
            if (
                killedSelf ||
                role.RemainingKills == 0 ||
                !CustomGameOptions.AssassinMultiKill
            )
            {
                HideButtons(role);
                return;
            }

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
        }


        public static void Prefix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin)) return;
            var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
            HideButtons(assassin);
        }
    }
}
