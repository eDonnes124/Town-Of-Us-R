using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public class GuesserPatchs
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        [HarmonyPrefix]

        private static void Prefix()
        {
            if (Role.GetRole(PlayerControl.LocalPlayer) is IGuesser guesser) guesser.HideAllButtons(guesser);
            if (Ability.GetAbility(PlayerControl.LocalPlayer) is IGuesser Guesser) Guesser.HideAllButtons(Guesser);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        [HarmonyPostfix]

        private static void Postfix()
        {
            if (Role.GetRole(PlayerControl.LocalPlayer) is IGuesser vigi)
            {
                if (!CustomGameOptions.VigilanteAfterVoting) vigi.HideAllButtons(vigi);
            }
            else  if (Role.GetRole(PlayerControl.LocalPlayer) is IGuesser doom)
            {
                if (!CustomGameOptions.DoomsayerAfterVoting) doom.HideAllButtons(doom);
            }
            else  if (Ability.GetAbility(PlayerControl.LocalPlayer) is IGuesser assassin)
            {
                if (!CustomGameOptions.AssassinateAfterVoting) assassin.HideAllButtons(assassin);
            }
        }
    }
}