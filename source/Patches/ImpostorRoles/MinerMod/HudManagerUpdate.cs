using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MinerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite MineSprite => TownOfUs.MineSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Miner)) return;
            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);

            __instance.AbilityButton.SetCoolDown(role.MineTimer(), CustomGameOptions.MineCd);
            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, role.VentSize, 0);
            hits = hits.ToArray().Where(c =>
                    (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                .ToArray();
            if (hits.Count == 0 && PlayerControl.LocalPlayer.moveable == true)
            {
                __instance.AbilityButton.graphic.color = Palette.EnabledColor;
                __instance.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
                role.CanPlace = true;
            }
            else
            {
                __instance.AbilityButton.graphic.color = Palette.DisabledClear;
                __instance.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
                role.CanPlace = false;
            }
        }
    }
}