using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.MercenaryMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MercMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            foreach(var role in Role.GetRoles(RoleEnum.Mercenary))
            {
                var merc = (Mercenary)role;
                if (merc.ShieldedPlayer != null && !merc.ShieldedPlayer.Data.Disconnected)
                {
                    merc.ShieldedPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    merc.ShieldedPlayer.myRend().material.SetFloat("_Outline", 0f);
                }
                merc.ShieldedPlayer = null;
            }
        }
    }
}