using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) return;
            var killButton = __instance.KillButton;
            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            killButton.gameObject.SetActive(role.canKill && !MeetingHud.Instance && (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled));

            killButton.graphic.color = Palette.EnabledColor;
            killButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}
