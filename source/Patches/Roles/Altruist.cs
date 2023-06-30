using AmongUs.GameOptions;
using Hazel;
using Reactor.Utilities;
using TownOfUs.CrewmateRoles.AltruistMod;
using UnityEngine;
using Coroutine = TownOfUs.CrewmateRoles.AltruistMod.Coroutine;

namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;

        public bool ReviveUsed;

        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            ImpostorText = () => "Sacrifice Yourself To Save Another";
            TaskText = () => "Revive a dead body at the cost of your own life";
            Color = Patches.Colors.Altruist;
            RoleType = RoleEnum.Altruist;
            AddToRoleHistory(RoleType);
        }

        protected override void HudManagerUpdate(HudManager __instance)
        {
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
                      maxDistance))
                {
                    continue;
                }

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            killButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            KillButtonTarget.SetTarget(killButton, closestBody, this);
            __instance.KillButton.SetCoolDown(0f, 1f);
        }

        protected override bool UseKillButton(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (this == null)
                return false;
            if (CurrentTarget == null)
                return false;
            if (Vector2.Distance(CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
            {
                return false;
            }

            var playerId = CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            if (player.IsInfected() || Player.IsInfected())
            {
                foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, Player);
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.AltruistRevive, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Coroutines.Start(Coroutine.AltruistRevive(CurrentTarget, this));
            return false;
        }
    }
}