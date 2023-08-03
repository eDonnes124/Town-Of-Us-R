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

            role.RoleAbilityButton.graphic.sprite = ExamineSprite;

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.ExamineMode)
            {
                role.RoleAbilityButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.ExamineCd);
                Utils.SetTarget(ref role.ClosestPlayer, role.RoleAbilityButton);

                var renderer = role.RoleAbilityButton.graphic;
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
            else
            {
                role.RoleAbilityButton.SetCoolDown(0f, 1f);
                var renderer = role.RoleAbilityButton.graphic;
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
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            (role as ITargetsDeadBody).SetTarget(killButton, closestBody, role);
            killButton.SetCoolDown(0f, 1f);
        }
    }
}