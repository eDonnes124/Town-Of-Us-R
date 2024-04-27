using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;

namespace TownOfUs.CultistRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            UpdateSwoopButton(__instance);
        }

        public static void UpdateSwoopButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon)) return;
            var swoopButton = __instance.KillButton;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            swoopButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !Utils.HasTask(TaskTypes.MushroomMixupSabotage));
            if (role.IsSwooped) swoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
            else swoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCd);

            var renderer = swoopButton.graphic;
            if (role.IsSwooped || !swoopButton.isCoolingDown)
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