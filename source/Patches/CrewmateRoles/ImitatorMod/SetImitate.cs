using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(MeetingHud))]
    public class SetImitate
    {
        public static PlayerVoteArea Imitate;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (Imitate == null) return;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Imitator))
                {
                    var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                    foreach (var button in imitator.Buttons.Where(button => button != null)) button.SetActive(false);

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == Imitate.TargetPlayerId) 
                        { 
                            imitator.ImitatePlayer = player;
                        }
                    }

                    if (Imitate == null)
                    {
                        Utils.Rpc(CustomRPC.Imitate, imitator.Player.PlayerId, sbyte.MaxValue);
                        return;
                    }

                    Utils.Rpc(CustomRPC.Imitate, imitator.Player.PlayerId, imitator.ImitatePlayer.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHud_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                Imitate = null;
            }
        }
    }
}