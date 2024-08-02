using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppSystem.Reflection;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using Reactor.Utilities;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudInvestigate
    {

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateInvestigateButton(__instance);
        }

        public static void UpdateInvestigateButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;

            var role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);

            if (role.ShouldNotify)
            {
                role.ShouldRemove = true;
                Coroutines.Start(Utils.FlashCoroutine(Color.cyan));
            }

            if (role.InvestigatedPlayer != null)
            {
            role.InvestigatedPlayer.myRend().material.SetColor("_VisorColor", Color.cyan);
            role.InvestigatedPlayer.nameText().color = Color.cyan;
            if (role.InvestigatedPlayer.Data.IsDead || role.InvestigatedPlayer.Data.Disconnected)
            {
                role.PreviouslyInvestigated.Add(role.InvestigatedPlayer.PlayerId);
                role.InvestigatedPlayer = null;
                Utils.Rpc(CustomRPC.StopInvestigation, role.Player.PlayerId);
                role.StartRemoving = false;
                role.ShouldNotify = false;
                role.ShouldRemove = false;
                MeetingStart.RoleList.Clear();
                MeetingStart.OnCrewRoles.Clear();
                MeetingStart.OnNeutRoles.Clear();
                MeetingStart.OnImpRoles.Clear();
                MeetingStart.SetRole = false;
                Coroutines.Start(Utils.FlashCoroutine(Color.red));
            }
            }

            var InvestigateButton = __instance.KillButton;

            InvestigateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            InvestigateButton.SetCoolDown(role.InvestigateTimer(), CustomGameOptions.InvestCd);
            Utils.SetTarget(ref role.ClosestPlayer, InvestigateButton, float.NaN);

            var renderer = InvestigateButton.graphic;
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