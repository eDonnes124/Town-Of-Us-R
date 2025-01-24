using System;
using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Modifiers.ShyMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            foreach (var modifier in Modifier.AllModifiers.Where(x => x.ModifierType == ModifierEnum.Shy))
            {
                var shy = (Shy)modifier;
                var player = shy.Player;
                if (!player.Data.IsDead && !player.Data.Disconnected && player.GetCustomOutfitType() == CustomPlayerOutfitType.Default && player.MyPhysics.body.velocity.magnitude == 0 && shy.Moving)
                {
                    shy.Moving = false;
                    shy.LastMoved = DateTime.UtcNow;
                }
                if (player.Data.Disconnected || shy.Moving) continue;
                if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Swooper)
                {
                    shy.Opacity = 0f;
                    if (PlayerControl.LocalPlayer.Data.IsImpostor() && player.Is(RoleEnum.Swooper) && !PlayerControl.LocalPlayer.IsHypnotised() ||
                        PlayerControl.LocalPlayer == player && player.Is(RoleEnum.Swooper)) shy.Opacity = 0.1f;
                    SetVisiblity(player, shy.Opacity, true);
                    shy.Moving = true;
                    continue;
                }
                else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
                {
                    shy.Opacity = 1f;
                    SetVisiblity(player, shy.Opacity, true);
                    shy.Moving = true;
                    continue;
                }
                else if (player.Data.IsDead || player.GetCustomOutfitType() == CustomPlayerOutfitType.Morph || player.MyPhysics.body.velocity.magnitude > 0)
                {
                    shy.Opacity = 1f;
                    SetVisiblity(player, shy.Opacity);
                    shy.Moving = true;
                    continue;
                }
                var timeSpan = DateTime.UtcNow - shy.LastMoved;
                if (timeSpan.TotalMilliseconds / 1000f < CustomGameOptions.InvisDelay) continue;
                else if (timeSpan.TotalMilliseconds / 1000f < CustomGameOptions.TransformInvisDuration + CustomGameOptions.InvisDelay)
                {
                    timeSpan = DateTime.UtcNow - shy.LastMoved.AddSeconds(CustomGameOptions.InvisDelay);
                    shy.Opacity = 1f - ((float)timeSpan.TotalMilliseconds / 1000f / CustomGameOptions.TransformInvisDuration * (100f - CustomGameOptions.FinalTransparency) / 100f);
                    SetVisiblity(player, shy.Opacity);
                    continue;
                }
                else
                {
                    shy.Opacity = CustomGameOptions.FinalTransparency / 100;
                    SetVisiblity(player, shy.Opacity);
                    continue;
                }
            }
        }

        public static void SetVisiblity(PlayerControl player, float transparency, bool swooped = false)
        {
            var colour = player.myRend().color;
            var cosmetics = player.cosmetics;
            colour.a = transparency;
            player.myRend().color = colour;
            if (swooped) transparency = 0f;
            cosmetics.nameText.color = cosmetics.nameText.color.SetAlpha(transparency);
            if (DataManager.Settings.Accessibility.ColorBlindMode) cosmetics.colorBlindText.color = cosmetics.colorBlindText.color.SetAlpha(transparency);
            player.SetHatAndVisorAlpha(transparency);
            cosmetics.skin.layer.color = cosmetics.skin.layer.color.SetAlpha(transparency);
            foreach (var rend in player.cosmetics.currentPet.renderers)
                rend.color = rend.color.SetAlpha(transparency);
            foreach (var shadow in player.cosmetics.currentPet.shadows)
                shadow.color = shadow.color.SetAlpha(transparency);
        }
    }
}