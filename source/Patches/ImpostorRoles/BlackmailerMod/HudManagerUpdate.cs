using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using AmongUs.GameOptions;

namespace TownOfUs.ImpostorRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Blackmail => TownOfUs.BlackmailSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer)) return;
            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);
            if (role.BlackmailButton == null)
            {
                role.BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlackmailButton.graphic.enabled = true;
                role.BlackmailButton.gameObject.SetActive(false);
            }

            role.BlackmailButton.graphic.sprite = Blackmail;
            role.BlackmailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);

            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(
                player => role.Blackmailed?.PlayerId != player.PlayerId
            ).ToList();

            Utils.SetTarget(ref role.ClosestPlayer, role.BlackmailButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notBlackmailed);

            role.BlackmailButton.SetCoolDown(role.BlackmailTimer(), CustomGameOptions.BlackmailCd);

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