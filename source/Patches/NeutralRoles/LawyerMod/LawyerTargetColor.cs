using HarmonyLib;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.LawyerMod
{
    public enum OnDefendantDead
    {
        Amnesiac,
        Survivor,
        Jester,
        Crew
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class LawyerTargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Lawyer role)
        {
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.color = new Color(0.93f, 0.7f, 0.55f, 1f);
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lawyer)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = Role.GetRole<Lawyer>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            role.target.nameText().color = new Color(0.93f, 0.7f, 0.55f, 1f);

            if (!role.target.Data.IsDead && !role.target.Data.Disconnected && !role.target.Is(RoleEnum.Vampire)) return;
            if (role.TargetVotedOut) return;

            Utils.Rpc(CustomRPC.LawyerToJester, PlayerControl.LocalPlayer.PlayerId);

            LwyrToJes(PlayerControl.LocalPlayer);
        }

        public static void LwyrToJes(PlayerControl player)
        {
            player.myTasks.RemoveAt(0);
            Role.RoleDictionary.Remove(player.PlayerId);


            if (CustomGameOptions.OnDefendantDead == OnDefendantDead.Jester)
            {
                var jester = new Jester(player);
                jester.SpawnedAs = false;
                jester.RegenTask();
            }
            else if (CustomGameOptions.OnDefendantDead == OnDefendantDead.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                amnesiac.SpawnedAs = false;
                amnesiac.RegenTask();
            }
            else if (CustomGameOptions.OnDefendantDead == OnDefendantDead.Survivor)
            {
                var surv = new Survivor(player);
                surv.SpawnedAs = false;
                surv.RegenTask();
            }
            else
            {
                new Crewmate(player);
            }
        }
    }
}