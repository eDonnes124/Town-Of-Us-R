using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.ImpostorRoles.GrenadierMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite FlashSprite => TownOfUs.FlashSprite;

        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier)) return;
            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);

            if (CustomGameOptions.GrenadierIndicators) 
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer && !player.Data.IsImpostor()) 
                    {
                        var tempColour = player.nameText().color;
                        var data = player?.Data;
                        if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                            continue;
                        if (role.flashedPlayers.Contains(player)) 
                        {
                            player.myRend().material.SetColor("_VisorColor", Color.black);
                            player.nameText().color = Color.black;
                        } 
                        else 
                        {
                            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                            player.nameText().color = tempColour;
                        }
                    }
                }
            }

            role.RoleAbilityButton.graphic.sprite = FlashSprite;

            if (role.Flashed)
            {
                role.RoleAbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.GrenadeDuration);
                return;
            }

            role.RoleAbilityButton.SetCoolDown(role.FlashTimer(), CustomGameOptions.GrenadeCd);

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var sabActive = specials.Any(s => s.IsActive);

            if (sabActive & !dummyActive)
            {
                role.RoleAbilityButton.graphic.color = Palette.DisabledClear;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 1f);
                return;
            }

            role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
            role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}