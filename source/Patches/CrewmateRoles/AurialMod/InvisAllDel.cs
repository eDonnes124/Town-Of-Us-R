using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.AurialMod
{
    public class InvisAllDel
    {
        [HarmonyPatch(typeof(MeetingHud),nameof(MeetingHud.Start))]
        public static class Aurial_MeetingStart
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
                {
                    var s = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                    s.NormalVision = true;
                    s.ClearEffect();
                    SeeAll.AllToNormal();
                }
                return;
            }
        }
    }
}
