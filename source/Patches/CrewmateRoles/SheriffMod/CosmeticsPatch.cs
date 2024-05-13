using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    public static class SheriffCosmeticsPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static void Postfix(PlayerControl __instance)
        {
            var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (CustomGameOptions.ShouldSheriffEnableCoolCosmetics)
            {  
            sheriff.Player.SetOutfit(CustomPlayerOutfitType.Sheriff, new GameData.PlayerOutfit()
            {
                ColorId = sheriff.Player.cosmetics.ColorId,
                HatId = "hat_Starless",
                VisorId = "visor_WinstonStache",
                SkinId = "skin_MilitaryDesert",
                PlayerName = sheriff.PlayerName,
            });
            }
        }
    }

}