using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Doomsayer))
            {
                var doomsayer = (Doomsayer)role;
                doomsayer.Guesses.Clear();
                doomsayer.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;

            var doomsayerRole = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer) as IGuesser;
            foreach (var voteArea in __instance.playerStates)
            {
                doomsayerRole.GenButton(doomsayerRole, voteArea);
            }
        }
    }
}
