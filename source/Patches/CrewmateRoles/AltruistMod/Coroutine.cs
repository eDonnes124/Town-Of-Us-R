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
using TownOfUs.Roles.Modifiers;
using AmongUs.GameOptions;
using Reactor.Utilities;

namespace TownOfUs.CrewmateRoles.AltruistMod
{
    public class Coroutine
    {
        public static Dictionary<PlayerControl, ArrowBehaviour> Revived = new();
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator AltruistRevive(DeadBody target, Altruist role)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                var lookout = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
                if (lookout.Watching.ContainsKey(target.ParentId))
                {
                    if (!lookout.Watching[target.ParentId].Contains(RoleEnum.Altruist)) lookout.Watching[target.ParentId].Add(RoleEnum.Altruist);
                }
            }

            var parent = Utils.PlayerById(target.ParentId);
            var position = target.TruePosition;
            var altruist = role.Player;

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

            if (!AmongUsClient.Instance.AmHost || parent.Data.Disconnected) yield break;

            AltruistReviveEnd(altruist, parent, position.x, position.y + 0.3636f);
            Utils.Rpc(CustomRPC.AltruistRevive, altruist.PlayerId, (byte)1, parent.PlayerId, position.x, position.y + 0.3636f);
        }

        public static void AltruistReviveEnd(PlayerControl altruist, PlayerControl player, float x, float y)
        {
            var revived = new List<PlayerControl>();

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == altruist.PlayerId) deadBody.gameObject.Destroy();
            }

            player.Revive();
            if (player.Is(Faction.Impostors)) RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.transform.position = new Vector2(x, y);
            if (PlayerControl.LocalPlayer == player) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(x, y));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }
            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == player.PlayerId) deadBody.gameObject.Destroy();
            }

            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Modifier.GetModifier<Lover>(player).OtherLover.Player;

                lover.Revive();
                if (lover.Is(Faction.Impostors)) RoleManager.Instance.SetRole(lover, RoleTypes.Impostor);
                else RoleManager.Instance.SetRole(lover, RoleTypes.Crewmate);
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                revived.Add(lover);

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        lover.transform.position = new Vector2(deadBody.TruePosition.x, deadBody.TruePosition.y + 0.3636f);
                        if (PlayerControl.LocalPlayer == lover) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(deadBody.TruePosition.x, deadBody.TruePosition.y + 0.3636f));

                        if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == lover.PlayerId)
                        {
                            Patches.SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
                        }
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

            if ((PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling)) && !revived.Contains(PlayerControl.LocalPlayer))
            {
                var gameObj = new GameObject();
                var Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Revived.Add(player, Arrow);
                //Target = player;
                Coroutines.Start(Utils.FlashCoroutine(Colors.Altruist, 1f, 0.5f));
            }

            foreach (var revive in revived) Utils.Unmorph(revive);
        }
    }
}