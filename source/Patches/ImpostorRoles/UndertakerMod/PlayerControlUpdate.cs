using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
            if (role.RoleAbilityButton == null)
            {
                role.RoleAbilityButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RoleAbilityButton.graphic.enabled = true;
                role.RoleAbilityButton.graphic.sprite = TownOfUs.DragSprite;
                role.RoleAbilityButton.gameObject.SetActive(false);
            }
            if (role.RoleAbilityButton.graphic.sprite != TownOfUs.DragSprite &&
                role.RoleAbilityButton.graphic.sprite != TownOfUs.DropSprite)
                role.RoleAbilityButton.graphic.sprite = TownOfUs.DragSprite;

            if (role.RoleAbilityButton.graphic.sprite == TownOfUs.DropSprite && role.CurrentlyDragging == null)
                role.RoleAbilityButton.graphic.sprite = TownOfUs.DragSprite;

            role.RoleAbilityButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);


            if (role.RoleAbilityButton.graphic.sprite == TownOfUs.DragSprite)
            {
                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                           PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                    LayerMask.GetMask(new[] {"Players", "Ghost"}));
                var killButton = role.RoleAbilityButton;
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
            }

            if (role.RoleAbilityButton.graphic.sprite == TownOfUs.DragSprite)
            {
                role.RoleAbilityButton.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCd);
            }
            else
            {
                role.RoleAbilityButton.SetCoolDown(0f, 1f);
                role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}