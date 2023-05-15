using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HUDProtect
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateProtectButton(__instance);
        }

        public static void UpdateProtectButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;

            var protectButton = __instance.KillButton;
            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);
            protectButton.SetCoolDown(role.StartTimer(), 10f);
            if (role.UsedAbility) return;
            Utils.SetTarget(ref role.ClosestPlayer, protectButton);
        }
    }
}
