using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.Patches;
using System.Linq;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerVentPatch
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if(__instance.ImpostorVentButton == null || __instance.ImpostorVentButton.gameObject == null || __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.CachedPlayerData) && !MeetingHud.Instance;
            if (active != __instance.ImpostorVentButton.gameObject.active)
            __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static bool CanVent(PlayerControl player, NetworkedPlayerInfo playerInfo)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return false;

            if (player.inVent)
            {
                if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2 && !player.Is(RoleEnum.Haunter) && !player.Is(RoleEnum.Phantom))
                {
                    player.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    player.MyPhysics.ExitAllVents();
                }
                return true;
            }

            if (playerInfo.IsDead)
                return false;

            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2) return false;

            if (player.Is(RoleEnum.Morphling) && !CustomGameOptions.MorphlingVent
                || player.Is(RoleEnum.Swooper) && !CustomGameOptions.SwooperVent
                || player.Is(RoleEnum.Grenadier) && !CustomGameOptions.GrenadierVent
                || player.Is(RoleEnum.Undertaker) && !CustomGameOptions.UndertakerVent
                || player.Is(RoleEnum.Escapist) && !CustomGameOptions.EscapistVent
                || player.Is(RoleEnum.Bomber) && !CustomGameOptions.BomberVent
                || (player.Is(RoleEnum.Undertaker) && Role.GetRole<Undertaker>(player).CurrentlyDragging != null && !CustomGameOptions.UndertakerVentWithBody))
                return false;

            if (player.Is(RoleEnum.Engineer) ||
                (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) || (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) ||
                (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) || (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) ||
                (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent))
                return true;

            if (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent)
            {
                var role = Role.GetRole<Werewolf>(player);
                if (role.Rampaged) return true;
            }

            return playerInfo.IsImpostor();
        }

        public static void Postfix(Vent __instance,
            [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;

            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) couldUse = CanVent(playerControl, playerInfo) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove || playerControl.inVent);
            else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && playerControl.Data.IsImpostor()) couldUse = false;
            else couldUse = canUse;

            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
            {
                couldUse = false;
            }

            canUse = couldUse;

            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);

                if (__instance.Id == 14 && SubmergedCompatibility.isSubmerged())
                    canUse &= (double)num <= (double)__instance.UsableDistance;
                else
                    canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
                
            }

            __result = num;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class JesterEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
                return false;
            return true;
        }
    }
}