using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.VigilanteMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Vigilante))
            {
                var retributionist = (Vigilante)role;
                retributionist.Guesses.Clear();
                retributionist.Buttons.Clear();
                retributionist.GuessedThisMeeting = false;
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante)) return;

            var retributionistRole = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer) as IRoleGuesser<Vigilante>;
            if (retributionistRole.Instance.RemainingKills <= 0) return;
            foreach (var voteArea in __instance.playerStates)
            {
                retributionistRole.GenButton(retributionistRole, voteArea);
            }
        }
    }
}
