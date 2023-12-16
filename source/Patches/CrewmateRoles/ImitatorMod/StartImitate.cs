using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Patches;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => StartImitate.ExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class StartImitate
    {
        public static PlayerControl ImitatingPlayer;
        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
            if (exiled == PlayerControl.LocalPlayer) return;

            var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitator.ImitatePlayer == null) return;

            Imitate(imitator);

            Utils.Rpc(CustomRPC.StartImitate, imitator.Player.PlayerId);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        public static void Imitate(Imitator imitator)
        {
            if (imitator.ImitatePlayer == null) return;
            ImitatingPlayer = imitator.Player;
            var imitatorRole = Role.GetRole(imitator.ImitatePlayer).RoleType;
            if (imitatorRole == RoleEnum.Haunter)
            {
                var haunter = Role.GetRole<Haunter>(imitator.ImitatePlayer);
                imitatorRole = haunter.formerRole;
            }
            if (imitatorRole == RoleEnum.Crewmate) return;
            var role = Role.GetRole(ImitatingPlayer);
            var killsList = (role.Kills, role.CorrectKills, role.IncorrectKills, role.CorrectAssassinKills, role.IncorrectAssassinKills);
            Role.RoleDictionary.Remove(ImitatingPlayer.PlayerId);
            if (imitatorRole == RoleEnum.Detective) new Detective(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Investigator) new Investigator(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Mystic) new Mystic(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Seer) new Seer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Spy) new Spy(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Tracker) new Tracker(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Sheriff) new Sheriff(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Veteran) new Veteran(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Altruist) new Altruist(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Engineer) new Engineer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medium) new Medium(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Transporter) new Transporter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Trapper) new Trapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Oracle) new Oracle(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medic)
            {
                var medic = new Medic(ImitatingPlayer);
                medic.UsedAbility = true;
                medic.StartingCooldown = medic.StartingCooldown.AddSeconds(-10f);
            }
            if (imitatorRole == RoleEnum.VampireHunter)
            {
                var vh = new VampireHunter(ImitatingPlayer);
                vh.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                vh.AddedStakes = true;
            }
            if (imitatorRole == RoleEnum.Aurial)
            {
                var aurial = new Aurial(ImitatingPlayer);
                aurial.CannotSeeDelay = DateTime.UtcNow;
                aurial.Loaded = true;
            }

            var newRole = Role.GetRole(ImitatingPlayer);
            newRole.RemoveFromRoleHistory(newRole.RoleType);
            newRole.Kills = killsList.Kills;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectKills = killsList.IncorrectKills;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
        }
    }
}