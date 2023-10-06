using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudConfess
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Oracle)) return;
            var confessButton = __instance.KillButton;

            var role = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);

            confessButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            confessButton.SetCoolDown(role.ConfessTimer(), CustomGameOptions.ConfessCd);

            var notConfessing = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Confessor)
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, confessButton, float.NaN, notConfessing);

            var renderer = confessButton.graphic;

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
