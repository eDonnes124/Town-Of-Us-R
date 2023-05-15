using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudInvestigate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateInvButton(__instance);
        }

        public static void UpdateInvButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            var investigateButton = __instance.KillButton;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            investigateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);
            investigateButton.SetCoolDown(role.SeerTimer(), CustomGameOptions.SeerCd);

            var notInvestigated = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Investigated.Contains(x.PlayerId))
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, investigateButton, float.NaN, notInvestigated);

            var renderer = investigateButton.graphic;

            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
