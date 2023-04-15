using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer)) return;
            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(
                player => role.Blackmailed?.PlayerId != player.PlayerId
            ).ToList();

            var player = PlayerControl.LocalPlayer;
            role.ClosestPlayer = Utils.SetClosestPlayer(ref player,
                                                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance],
                                                        notBlackmailed);
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

            if (role.Blackmailed != null && !role.Blackmailed.Data.IsDead && !role.Blackmailed.Data.Disconnected)
            {
                role.Blackmailed.myRend().material.SetFloat("_Outline", 1f);
                role.Blackmailed.myRend().material.SetColor("_OutlineColor", new Color(0.3f, 0.0f, 0.0f));
                if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                    role.Blackmailed.nameText().color = new Color(0.3f, 0.0f, 0.0f);
                else role.Blackmailed.nameText().color = Color.clear;
            }

            var imps = PlayerControl.AllPlayerControls.ToArray().Where(
                player => player.Data.IsImpostor() && player != role.Blackmailed
            ).ToList();

            foreach (var imp in imps)
            {
                if ((imp.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage ||
                    imp.GetCustomOutfitType() == CustomPlayerOutfitType.Swooper)) imp.nameText().color = Color.clear;
                else if (imp.nameText().color == Color.clear ||
                    imp.nameText().color == new Color(0.3f, 0.0f, 0.0f)) imp.nameText().color = Patches.Colors.Impostor;
            }
        }
    }
}