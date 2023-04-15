using HarmonyLib;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer)) return true;
            if (RoleManager.IsGhostRole(PlayerControl.LocalPlayer.Data.RoleType)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
            if (__instance.isCoolingDown) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.BlackmailTimer() != 0) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                role.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                if (role.Blackmailed != null && role.Blackmailed.Data.IsImpostor())
                {
                    if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        role.Blackmailed.nameText().color = Patches.Colors.Impostor;
                    else role.Blackmailed.nameText().color = Color.clear;
                }
                role.Blackmailed = target;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.Blackmail, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            __instance.SetCoolDown(0f, 1f);
            return false;
        }
    }
}