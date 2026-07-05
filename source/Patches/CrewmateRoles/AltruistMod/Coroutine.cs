using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Patches;
using UnityEngine;
using AmongUs.GameOptions;
using Reactor.Utilities;
using Hazel;
using Reactor.Networking.Extensions;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.Modifiers;

namespace TownOfUs.CrewmateRoles.AltruistMod
{
    public class Coroutine
    {
        public static Dictionary<PlayerControl, ArrowBehaviour> Revived = new();
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator AltruistRevive(Altruist role)
        {
            var altruist = role.Player;
            if (PlayerControl.LocalPlayer == altruist) Coroutines.Start(Utils.FlashCoroutine(role.Color, CustomGameOptions.ReviveDuration));
            altruist.RemainingEmergencies = 0;
            role.UsesLeft--;
            role.UsedThisRound = true;
            role.CurrentlyReviving = true;

            var startTime = DateTime.UtcNow;

            if ((PlayerControl.LocalPlayer.Is(Faction.Impostors) || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))
                && !PlayerControl.LocalPlayer.Data.IsDead) {
                Coroutines.Start(Utils.FlashCoroutine(role.Color));
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = altruist.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                arrow.image = renderer;
                gameObj.layer = 5;
                role.Arrows.Add(arrow);
            }

            while (true)
            {
                foreach (var arrow in role.Arrows) arrow.target = altruist.transform.position;

                if (MeetingHud.Instance || !altruist.Is(RoleEnum.Altruist) || altruist.Data.IsDead || altruist.Data.Disconnected)
                {
                    altruist.moveable = true;
                    role.CurrentlyReviving = false;
                    role.Arrows.DestroyAll();
                    role.Arrows.Clear();
                    yield break;
                }

                altruist.MyPhysics.body.velocity = new Vector2 (0f, 0f);
                altruist.moveable = false;
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;
                role.TimeRemaining = ((float)seconds - CustomGameOptions.ReviveDuration) * -1;
                if (role.TimeRemaining < 0) role.TimeRemaining = 0;
                if (seconds < CustomGameOptions.ReviveDuration && (AmongUsClient.Instance.AmHost || PlayerControl.LocalPlayer != altruist)) yield return null;
                else if (seconds < CustomGameOptions.ReviveDuration - 0.1f) yield return null;
                else break;
            }

            altruist.moveable = true;
            role.CurrentlyReviving = false;
            role.Arrows.DestroyAll();
            role.Arrows.Clear();

            if (altruist != PlayerControl.LocalPlayer || altruist.Data.IsDead || altruist.Data.Disconnected) yield break;

            var revives = GetRevivals(altruist.GetTruePosition());

            ReviveRPC(altruist, revives);
        }

        private static Dictionary<byte, Vector2> GetRevivals(Vector2 refPosition)
        {
            var deadBodies = GameObject.FindObjectsOfType<DeadBody>();
            Dictionary<byte, Vector2> revives = new Dictionary<byte, Vector2>(deadBodies.Count);
            foreach (var body in deadBodies)
            {
                Vector2 playerPosition = body.transform.localPosition;
                var canRevive = Vector2.Distance(refPosition, playerPosition) <= CustomGameOptions.ReviveRadius * ShipStatus.Instance.MaxLightRadius;
                if (!canRevive) continue;
                revives.Add(body.ParentId, new Vector2(body.TruePosition.x, body.TruePosition.y));
            }
            return revives;
        }

        public static void ReviveRPC(PlayerControl altruist, Dictionary<byte, Vector2> revives)
        {
            byte isHost = AmongUsClient.Instance.AmHost ? (byte)1 : (byte)2;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.AltruistRevive,
                SendOption.Reliable,
                -1);
            writer.Write(altruist.PlayerId);
            writer.Write(isHost);
            writer.Write((byte)revives.Count);
            foreach ((byte key, Vector2 value) in revives)
            {
                writer.Write(key);
                writer.Write(value);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            if (AmongUsClient.Instance.AmHost) AltruistReviveEnd(altruist, revives);
        }

        public static void AltruistReviveEnd(PlayerControl altruist, Dictionary<byte, Vector2> revives)
        {
            if ((PlayerControl.LocalPlayer.Is(Faction.Impostors) || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))
                && !PlayerControl.LocalPlayer.Data.IsDead) Coroutines.Start(Utils.FlashCoroutine(Colors.Altruist));

            var revived = new List<PlayerControl>();

            foreach ((byte key, Vector2 value) in revives)
            {
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == key) deadBody.gameObject.Destroy();
                }

                var player = Utils.PlayerById(key);

                if (PlayerControl.LocalPlayer == player) Coroutines.Start(Utils.FlashCoroutine(Colors.Altruist));
                player.Revive();
                if (player.Is(Faction.Impostors)) RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
                else RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
                revived.Add(player);
                var position = new Vector2(value.x, value.y + 0.3636f);
                player.transform.position = new Vector2(position.x, position.y);
                if (PlayerControl.LocalPlayer == player) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(position.x, position.y));

                if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
                }

                if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))
                {
                    var gameObj = new GameObject();
                    var Arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    Arrow.image = renderer;
                    gameObj.layer = 5;
                    Revived.Add(player, Arrow);
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

            foreach (var revive in revived)
            {
                if (revive.IsLover() && CustomGameOptions.BothLoversDie)
                {
                    var lover = Modifier.GetModifier<Lover>(revive).OtherLover.Player;

                    if (lover.Data.IsDead)
                    {
                        lover.Revive();
                        if (lover.Is(Faction.Impostors)) RoleManager.Instance.SetRole(lover, RoleTypes.Impostor);
                        else RoleManager.Instance.SetRole(lover, RoleTypes.Crewmate);
                        Murder.KilledPlayers.Remove(
                            Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));

                        Utils.Unmorph(lover);
                        lover.RemainingEmergencies = 0;
                        if (lover == SetHaunter.WillBeHaunter && AmongUsClient.Instance.AmHost)
                        {
                            SetHaunter.WillBeHaunter = null;
                            Utils.Rpc(CustomRPC.SetHaunter, byte.MaxValue);
                        }
                        if (lover == SetPhantom.WillBePhantom && AmongUsClient.Instance.AmHost)
                        {
                            SetPhantom.WillBePhantom = null;
                            Utils.Rpc(CustomRPC.SetPhantom, byte.MaxValue);
                        }

                        foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                        {
                            if (deadBody.ParentId == lover.PlayerId)
                            {
                                deadBody.gameObject.Destroy();
                            }
                        }

                        lover.transform.position = new Vector2(revive.transform.localPosition.x, revive.transform.localPosition.y);
                        if (PlayerControl.LocalPlayer == lover) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(revive.transform.localPosition.x, revive.transform.localPosition.y));

                        if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == lover.PlayerId)
                        {
                            Patches.SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
                        }

                        if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))
                        {
                            var gameObj = new GameObject();
                            var Arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Sprite;
                            Arrow.image = renderer;
                            gameObj.layer = 5;
                            Revived.Add(lover, Arrow);
                        }
                    }
                }

                Utils.Unmorph(revive);
                revive.RemainingEmergencies = 0;
                if (revive == SetHaunter.WillBeHaunter && AmongUsClient.Instance.AmHost)
                {
                    SetHaunter.WillBeHaunter = null;
                    Utils.Rpc(CustomRPC.SetHaunter, byte.MaxValue);
                }
                if (revive == SetPhantom.WillBePhantom && AmongUsClient.Instance.AmHost)
                {
                    SetPhantom.WillBePhantom = null;
                    Utils.Rpc(CustomRPC.SetPhantom, byte.MaxValue);
                }
                if (revive.Is(RoleEnum.Jester))
                {
                    var jest = Role.GetRole<Jester>(revive);
                    jest.LastMoved = DateTime.UtcNow;
                }
                if (revive.Is(RoleEnum.Survivor))
                {
                    var surv = Role.GetRole<Survivor>(revive);
                    surv.LastMoved = DateTime.UtcNow;
                }
                if (revive.Is(ModifierEnum.Celebrity))
                {
                    var celeb = Modifier.GetModifier<Celebrity>(revive);
                    celeb.JustDied = false;
                }
            }
        }
    }
}