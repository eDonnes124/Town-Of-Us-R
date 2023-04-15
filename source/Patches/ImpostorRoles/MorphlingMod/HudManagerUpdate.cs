using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SampleSprite => TownOfUs.SampleSprite;
        public static Sprite MorphSprite => TownOfUs.MorphSprite;


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) return;
            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (__instance.AbilityButton.graphic.sprite != SampleSprite && __instance.AbilityButton.graphic.sprite != MorphSprite)
                __instance.AbilityButton.graphic.sprite = SampleSprite;

            if (__instance.AbilityButton.graphic.sprite == SampleSprite)
            {
                __instance.AbilityButton.SetCoolDown(0f, 1f);
                var player = PlayerControl.LocalPlayer;
                role.ClosestPlayer = Utils.SetClosestPlayer(ref player, 
                                                            GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], 
                                                            PlayerControl.AllPlayerControls.ToArray().ToList());
                if (role.ClosestPlayer != null)
                {
                    __instance.AbilityButton.graphic.color = Palette.EnabledColor;
                    __instance.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
                }
                else if (role.ClosestPlayer == null)
                {
                    __instance.AbilityButton.graphic.color = Palette.DisabledClear;
                    __instance.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
                }
            }
            else
            {
                if (role.Morphed)
                {
                    __instance.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                __instance.AbilityButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                __instance.AbilityButton.graphic.color = Palette.EnabledColor;
                __instance.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
