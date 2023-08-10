using HarmonyLib;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles;

namespace TownOfUs.Modifiers.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Ability.GetAbilities(AbilityEnum.Assassin))
            {
                var assassin = (Assassin) role;
                assassin.Guesses.Clear();
                assassin.Buttons.Clear();
                assassin.GuessedThisMeeting = false;
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin)) return;
            if (PlayerControl.LocalPlayer.Is(Faction.NeutralBenign)) return;

            var assassinRole = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer) as IAbilityGuesser<Assassin>;
            if (assassinRole.Instance.RemainingKills <= 0) return;
            foreach (var voteArea in __instance.playerStates)
            {
                assassinRole.GenButton(assassinRole, voteArea);
            }
        }
    }
}
