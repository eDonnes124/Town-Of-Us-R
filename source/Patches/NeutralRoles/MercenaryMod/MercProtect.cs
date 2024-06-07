using HarmonyLib;
using Hazel;
using Il2CppSystem;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MercenaryMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class MercProtect
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary);
            if (!flag) return true;
            var role = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (__instance == role.ArmorButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (__instance.isCoolingDown) return false;
                if (role.ArmorTimer() != 0) return false;
                role.TimeRemaining = CustomGameOptions.ArmorDuration;
                role.Brilders -= 1;
                if (role.ShieldedPlayer != null)
                {
                    role.ShieldedPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    role.ShieldedPlayer.myRend().material.SetFloat("_Outline", 0f);
                }
                role.ShieldedPlayer = null;
                role.DonArmor();
                role.RegenTask();
                Utils.Rpc(CustomRPC.DonArmor, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (role.ClosestPlayer == null) return false;
            if (role.ShieldedPlayer == role.ClosestPlayer) return false;
            if (role.StartTimer() > 0) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if (role.ShieldedPlayer != null)
                {
                    role.ShieldedPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    role.ShieldedPlayer.myRend().material.SetFloat("_Outline", 0f);
                }
                Utils.Rpc(CustomRPC.MercProtect, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);

                role.ShieldedPlayer = role.ClosestPlayer;
                role.StartingCooldown = System.DateTime.UtcNow;
            }
            else if (interact[5] == true)
            {
                role.StartingCooldown = System.DateTime.UtcNow;
                role.StartingCooldown = role.StartingCooldown.AddSeconds(CustomGameOptions.ProtectAbsorbCd - CustomGameOptions.ProtectCd);
            }
            return false;
        }
    }
}
