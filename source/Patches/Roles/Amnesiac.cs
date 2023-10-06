using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.NeutralRoles.AmnesiacMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public bool SpawnedAs = true;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            ImpostorText = () => "Remember A Role Of A Deceased Player";
            TaskText = () => SpawnedAs ? "Find a dead body to remember a role" : "Your target was killed. Now remember a new role!";
            Color = Patches.Colors.Amnesiac;
            RoleType = RoleEnum.Amnesiac;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralBenign;
        }

        public DeadBody CurrentTarget;

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var amnesiacTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            amnesiacTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = amnesiacTeam;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
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

            var killButton = __instance.KillButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();

            foreach (var body in allBodies.Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);
                if (!(distance < closestDistance)) continue;

                closestBody = body;
                closestDistance = distance;
            }

            if (CustomGameOptions.RememberArrows && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x =>
                    Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.RememberArrowDelay) < System.DateTime.UtcNow));

                foreach (var bodyArrow in BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    {
                        DestroyArrow(bodyArrow);
                    }
                }

                foreach (var body in validBodies)
                {
                    if (!BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = TownOfUs.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        BodyArrows.Add(body.ParentId, arrow);
                    }
                    BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else
            {
                if (BodyArrows.Count != 0)
                {
                    BodyArrows.Values.DestroyAll();
                    BodyArrows.Clear();
                }
            }

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            KillButtonTarget.SetTarget(killButton, closestBody, this);
            __instance.KillButton.SetCoolDown(0f, 1f);
        }

        protected override bool UseKillButton(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);
            if (!flag) return true;
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
            if ((player.IsInfected() || Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, Player);
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Remember, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            PerformKillButton.Remember(this, player);
            return false;
        }
    }
}