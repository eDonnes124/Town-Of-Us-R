using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Roles.Modifiers;
using AmongUs.GameOptions;
using TownOfUs.Patches;
using Hazel;

namespace TownOfUs.CrewmateRoles.AltruistMod
{
    public class Coroutine
    {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator AltruistRevive(DeadBody target, Altruist role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;

            var revived = new List<PlayerControl>();

            if (AmongUsClient.Instance.AmHost) Utils.RpcMurderPlayer(role.Player, role.Player);

            if (CustomGameOptions.AltruistTargetBody)
                if (target != null)
                {
                    foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                    {
                        if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                    }
                }

            var startTime = DateTime.UtcNow;
            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;
                if (seconds < CustomGameOptions.ReviveDuration)
                    yield return null;
                else break;

                if (MeetingHud.Instance) yield break;
            }

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == role.Player.PlayerId) deadBody.gameObject.Destroy();
            }

            var player = Utils.PlayerById(parentId);

            player.Revive();
            if (player.Is(Faction.Impostors)) RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }
            if (target != null) Object.Destroy(target.gameObject);

            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Modifier.GetModifier<Lover>(player).OtherLover.Player;

                lover.Revive();
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                revived.Add(lover);

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        deadBody.gameObject.Destroy();
                    }
                }
            }

            if (revived.Any(x => x.AmOwner))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }

            ExilePatch.CheckTraitorSpawn(null);
            if (AmongUsClient.Instance.AmHost)
            {
                if (ExilePatch.WillBeHaunter == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected && x != player).ToList();
                    if (toChooseFrom.Count == 0)
                    {
                        ExilePatch.WillBeHaunter = null;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
                        writer.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    else
                    {
                        var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];

                        ExilePatch.WillBeHaunter = pc;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
                        writer.Write(pc.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
                if (ExilePatch.WillBePhantom == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.NeutralOther) || x.Is(Faction.NeutralKilling)) && !x.Is(ModifierEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected && x != player).ToList();
                    if (toChooseFrom.Count == 0)
                    {
                        ExilePatch.WillBePhantom = null;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                        writer.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    else
                    {
                        var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];

                        ExilePatch.WillBePhantom = pc;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                        writer.Write(pc.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)
                || PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) || PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf)
                || PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
            {
                var gameObj = new GameObject();
                Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Target = player;
                yield return Utils.FlashCoroutine(role.Color, 1f, 0.5f);
            }
        }
    }
}