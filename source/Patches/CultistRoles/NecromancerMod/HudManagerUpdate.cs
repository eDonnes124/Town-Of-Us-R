using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CultistRoles.NecromancerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ReviveHudManagerUpdate
    {
        public static Sprite ReviveSprite => TownOfUs.Revive2Sprite;
        public static byte DontRevive = byte.MaxValue;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer)) return;
            var role = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);

            role.RoleAbilityButton.graphic.sprite = ReviveSprite;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

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

            role.RoleAbilityButton.SetCoolDown(role.ReviveTimer(),
                CustomGameOptions.ReviveCooldown + CustomGameOptions.IncreasedCooldownPerRevive * role.ReviveCount);

            if (role.CurrentTarget && role.CurrentTarget != closestBody)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (closestBody != null && closestBody.ParentId == DontRevive) closestBody = null;
            role.CurrentTarget = closestBody;
            if (role.CurrentTarget == null)
            {
                role.RoleAbilityButton.graphic.color = Palette.DisabledClear;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 1f);
                return;
            }
            var player = Utils.PlayerById(role.CurrentTarget.ParentId);
            if (role.CurrentTarget && role.RoleAbilityButton.enabled &&
                !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.CultistSeer) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Mayor)) &&
                !(PlayerControl.LocalPlayer.killTimer > GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - 0.5f))
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.red);
                role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
                role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            role.RoleAbilityButton.graphic.color = Palette.DisabledClear;
            role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 1f);
        }
    }
}