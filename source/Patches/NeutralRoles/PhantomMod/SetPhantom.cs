using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Patches;

namespace TownOfUs.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetPhantom.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetPhantom
    {
        public static PlayerControl WillBePhantom;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (WillBePhantom == null) return;
            var exiled = __instance.exiled?.Object;
            if (!WillBePhantom.Data.IsDead && (exiled.Is(Faction.NeutralKilling) || exiled.Is(Faction.NeutralEvil) || exiled.Is(Faction.NeutralBenign)) && !exiled.IsLover()) WillBePhantom = exiled;
            if (exiled == WillBePhantom && exiled.Is(RoleEnum.Jester)) return;
            if (WillBePhantom.Data.Disconnected) return;
            if (!WillBePhantom.Data.IsDead && WillBePhantom != exiled) return;

            if (!WillBePhantom.Is(RoleEnum.Phantom))
            {
                var oldRole = Role.GetRole(WillBePhantom);
                var killsList = (oldRole.Kills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBePhantom.PlayerId);
                if (PlayerControl.LocalPlayer == WillBePhantom)
                {
                    var role = new Phantom(PlayerControl.LocalPlayer);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.RegenTask();
                }
                else
                {
                    var role = new Phantom(WillBePhantom);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                }

                Utils.RemoveTasks(WillBePhantom);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter)) WillBePhantom.MyPhysics.ResetMoveState();

                WillBePhantom.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            if (PlayerControl.LocalPlayer != WillBePhantom) return;

            if (Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught) return;
            var startingVent =
                ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

            Utils.Rpc(CustomRPC.SetPos, PlayerControl.LocalPlayer.PlayerId, startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);

            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 5) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}