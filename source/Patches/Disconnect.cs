using HarmonyLib;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(GameData))]
    public class DisconnectHandler
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (CustomGameOptions.GameMode == GameMode.Cultist)
            {
                if (player.Is(RoleEnum.Necromancer) || player.Is(RoleEnum.Whisperer))
                {
                    foreach (var player2 in PlayerControl.AllPlayerControls)
                    {
                        if (player2.Is(Faction.Impostors)) Utils.MurderPlayer(player2, player2, true);
                    }
                }
            }
            else
            {
                ExilePatch.CheckTraitorSpawn(player);
                /*if (player.IsLover())
                {
                    var otherLover = Modifier.GetModifier<Lover>(player).OtherLover;
                    if (!otherLover.Is(RoleEnum.Pestilence) && !otherLover.Data.IsDead
                         && !otherLover.Data.Disconnected) MurderPlayer(otherLover, otherLover, true);
                    if (otherLover.Is(RoleEnum.Sheriff))
                    {
                        var sheriff = Role.GetRole<Sheriff>(otherLover);
                        sheriff.IncorrectKills -= 1;
                    }
                }*/
            }
        }
    }
}