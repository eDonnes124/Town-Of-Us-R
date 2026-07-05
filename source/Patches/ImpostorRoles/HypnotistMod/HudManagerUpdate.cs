using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;

namespace TownOfUs.ImpostorRoles.HypnotistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Hypnotise => TownOfUs.HypnotiseSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hypnotist)) return;
            var role = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
            if (role.HypnotiseButton == null)
            {
                role.HypnotiseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HypnotiseButton.graphic.enabled = true;
                role.HypnotiseButton.gameObject.SetActive(false);
            }
            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var playerId in role.HypnotisedPlayers)
                {
                    var player = Utils.PlayerById(playerId);
                    var data = player?.Data;
                    if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead || playerId == PlayerControl.LocalPlayer.PlayerId)
                        continue;

                    if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                    {
                        var colour = new Color(0.6f, 0f, 0f);
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    else player.nameText().color = Color.clear;
                }
            }

            if (PlayerControl.LocalPlayer.Data.IsDead || role.HysteriaActive) role.HypnotiseButton.SetTarget(null);

            role.HypnotiseButton.graphic.sprite = Hypnotise;
            role.HypnotiseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead && !role.HysteriaActive
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notHypnotised = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !role.HypnotisedPlayers.Contains(player.PlayerId)
            ).ToList();

            role.HypnotiseButton.SetCoolDown(role.HypnotiseTimer(), CustomGameOptions.HypnotiseCd);
            if (!PlayerControl.LocalPlayer.moveable) role.HypnotiseButton.SetTarget(null);
            else if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, role.HypnotiseButton, float.NaN, notHypnotised);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, role.HypnotiseButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !role.HypnotisedPlayers.Contains(x.PlayerId)).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, role.HypnotiseButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !role.HypnotisedPlayers.Contains(x.PlayerId)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, role.HypnotiseButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !role.HypnotisedPlayers.Contains(x.PlayerId)).ToList());
        }
    }
}