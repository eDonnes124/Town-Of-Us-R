using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Patches;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetHaunter.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetHaunter
    {
        public static PlayerControl WillBeHaunter;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (WillBeHaunter == null) return;
            var exiled = __instance.exiled?.Object;
            if (!WillBeHaunter.Data.IsDead && exiled.Is(Faction.Crewmates) && !exiled.IsLover()) WillBeHaunter = exiled;
            if (WillBeHaunter.Data.Disconnected) return;
            if (!WillBeHaunter.Data.IsDead && WillBeHaunter != exiled) return;

            if (!WillBeHaunter.Is(RoleEnum.Haunter))
            {
                var oldRole = Role.GetRole(WillBeHaunter);
                var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBeHaunter.PlayerId);
                if (PlayerControl.LocalPlayer == WillBeHaunter)
                {
                    var role = new Haunter(PlayerControl.LocalPlayer);
                    role.formerRole = oldRole.RoleType;
                    role.CorrectKills = killsList.CorrectKills;
                    role.IncorrectKills = killsList.IncorrectKills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.RegenTask();
                }
                else
                {
                    var role = new Haunter(WillBeHaunter);
                    role.formerRole = oldRole.RoleType;
                    role.CorrectKills = killsList.CorrectKills;
                    role.IncorrectKills = killsList.IncorrectKills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                }

                Utils.RemoveTasks(WillBeHaunter);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom)) WillBeHaunter.MyPhysics.ResetMoveState();

                WillBeHaunter.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            if (PlayerControl.LocalPlayer != WillBeHaunter) return;

            if (Role.GetRole<Haunter>(PlayerControl.LocalPlayer).Caught) return;
            var startingVent =
                ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPos, SendOption.Reliable, -1);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(startingVent.transform.position.x);
            writer2.Write(startingVent.transform.position.y + 0.3636f);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5) return;
            if (obj.name.Contains("ExileCutscene")) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}