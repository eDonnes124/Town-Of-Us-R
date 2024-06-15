using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudExamine
    {
        public static Sprite ExamineSprite => TownOfUs.ExamineSprite;

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateExamineButton(__instance);
        }

        public static void UpdateExamineButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective)) return;

            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            if (role.ExamineButton == null)
            {
                role.ExamineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ExamineButton.graphic.enabled = true;
                role.ExamineButton.gameObject.SetActive(false);
            }

            role.ExamineButton.graphic.sprite = ExamineSprite;
            role.ExamineButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.ExamineButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.ExamineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.ExamineCd);
            Utils.SetTarget(ref role.ClosestPlayer, role.ExamineButton, float.NaN);

            var renderer = role.ExamineButton.graphic;
            if (role.ClosestPlayer != null && role.InvestigatingScene != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            var killButton = __instance.KillButton;
            CrimeScene closestScene = null;
            var closestDistance = float.MaxValue;
            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.gameObject.name != "CrimeScene") continue;
                var component = collider2D.GetComponent<CrimeScene>();
                if (component == null) continue;
                if (role.InvestigatingScene == component) continue;

                if (!(Vector2.Distance(truePosition, component.gameObject.transform.position) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.gameObject.transform.position);
                if (!(distance < closestDistance)) continue;
                closestScene = component;
                closestDistance = distance;
            }

            KillButtonTarget.SetTarget(killButton, closestScene, role);
            killButton.SetCoolDown(0f, 1f);
        }
    }
}