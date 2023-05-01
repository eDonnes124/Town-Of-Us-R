using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;
using System;
using Hazel;
using TownOfUs.Roles;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using AmongUs.GameOptions;
using Reactor.Utilities;
using TownOfUs.Roles.Modifiers;
using System.Collections.Generic;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => ExilePatch.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class ExilePatch
    {
        public static List<PlayerControl> AssassinatedPlayers = new List<PlayerControl>();
        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (PlayerControl.LocalPlayer.Data.Disconnected) return;
            foreach (var player in AssassinatedPlayers)
            {
                if (!player.Data.Disconnected) player.Exiled();
            }
            AssassinatedPlayers.Clear();
            var exiled = __instance.exiled?.Object;
            if (exiled != null)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Jester))
                {
                    if (exiled.PlayerId == ((Jester)role).Player.PlayerId) ((Jester)role).Wins();
                    return;
                }
                foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                {
                    if (exiled.PlayerId == ((Executioner)role).target.PlayerId) ((Executioner)role).Wins();
                    return;
                }
            }
            if (CustomGameOptions.GameMode == GameMode.Cultist) CultistExile(exiled);
            CheckTraitorSpawn(exiled);
            SetHaunter(exiled);
            SetPhantom(exiled);
            SetTraitor(exiled);
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) ArsonistLastKiller(exiled);
            if (exiled.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Modifier.GetModifier<Lover>(exiled).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence) && !otherLover.Data.IsDead
                     && !otherLover.Data.Disconnected) otherLover.Exiled();
            }
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5) return;
            if (obj.name.Contains("ExileCutscene")) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        public static void CultistExile(PlayerControl exiled)
        {
            var cultist = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => x.Is(RoleEnum.Necromancer) || x.Is(RoleEnum.Whisperer)).ToList();
            foreach (var cult in cultist)
            {
                if (cult.Data.IsDead || cult.Data.Disconnected)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.Data.IsImpostor()) Utils.MurderPlayer(player, player, true);
                    }
                }
            }
            if (exiled == null) return;
            if (exiled.Is(RoleEnum.Necromancer) || exiled.Is(RoleEnum.Whisperer))
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                        .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                foreach (var player in alives)
                {
                    if (player.Data.IsImpostor()) Utils.MurderPlayer(player, player, true);
                }
            }
        }

        public static bool HaunterOn;
        public static PlayerControl WillBeHaunter;
        public static void SetHaunter(PlayerControl exiled)
        {
            if (!HaunterOn) return;
            if (WillBeHaunter == null && exiled.Is(Faction.Crewmates) && !exiled.IsLover()) WillBeHaunter = exiled;
            if (WillBeHaunter != null && !WillBeHaunter.Is(RoleEnum.Haunter))
            {
                var oldRole = Role.GetRole(WillBeHaunter);
                var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBeHaunter.PlayerId);
                var role = new Haunter(WillBeHaunter);
                role.formerRole = oldRole.RoleType;
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();

                WillBeHaunter.gameObject.layer = LayerMask.NameToLayer("Players");
                RemoveTasks(WillBeHaunter);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom)) PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }
            if (WillBeHaunter != null && WillBeHaunter == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Haunter>(WillBeHaunter).Caught) return;
                var startingVent =
                    ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetPos, SendOption.Reliable, -1);
                writer.Write(WillBeHaunter.PlayerId);
                writer.Write(startingVent.transform.position.x);
                writer.Write(startingVent.transform.position.y + 0.3636f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                WillBeHaunter.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
            }
        }

        public static bool PhantomOn;
        public static PlayerControl WillBePhantom;
        public static void SetPhantom(PlayerControl exiled)
        {
            if (!PhantomOn) return;
            if (WillBePhantom == null && (exiled.Is(Faction.NeutralOther) || exiled.Is(Faction.NeutralKilling)) && !exiled.IsLover()) WillBePhantom = exiled;
            if (WillBePhantom != null && !WillBePhantom.Is(RoleEnum.Phantom))
            {
                var oldRole = Role.GetRole(WillBePhantom);
                var killsList = (oldRole.Kills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBePhantom.PlayerId);
                var role = new Phantom(WillBePhantom);
                role.Kills = killsList.Kills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();

                WillBePhantom.gameObject.layer = LayerMask.NameToLayer("Players");
                RemoveTasks(WillBePhantom);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter)) PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
            }
            if (WillBePhantom != null && WillBePhantom == PlayerControl.LocalPlayer)
            {
                if (Role.GetRole<Phantom>(WillBePhantom).Caught) return;
                var startingVent =
                    ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetPos, SendOption.Reliable, -1);
                writer.Write(WillBePhantom.PlayerId);
                writer.Write(startingVent.transform.position.x);
                writer.Write(startingVent.transform.position.y + 0.3636f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                WillBePhantom.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
            }
        }

        public static bool TraitorOn;
        public static bool TraitorCanSpawn;
        public static PlayerControl WillBeTraitor;
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void SetTraitor(PlayerControl exiled)
        {
            if (!TraitorOn || !TraitorCanSpawn) return;
            if (AmongUsClient.Instance.AmHost && WillBeTraitor == null)
            {
                var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) &&
                !x.Is(ModifierEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected && x != exiled && !x.IsExeTarget()).ToList();
                var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFrom.Count);
                var pc = toChooseFrom[rand];

                WillBeTraitor = pc;

                if (WillBeTraitor == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

                var oldRole = Role.GetRole(WillBeTraitor);
                var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBeTraitor.PlayerId);
                var role = new Traitor(WillBeTraitor);
                role.formerRole = oldRole.RoleType;
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();

                SpawnTraitor(WillBeTraitor);

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetTraitor, SendOption.Reliable, -1);
                writer.Write(pc.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public static void SpawnTraitor(PlayerControl player)
        {
            if (PlayerControl.LocalPlayer == player)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                {
                    var snitchRole = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
                    snitchRole.ImpArrows.DestroyAll();
                    snitchRole.SnitchArrows.Values.DestroyAll();
                    snitchRole.SnitchArrows.Clear();
                    CompleteTask.Postfix(PlayerControl.LocalPlayer);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                {
                    var engineerRole = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(engineerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    UnityEngine.Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                {
                    var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(transporterRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(veteranRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                {
                    var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(trapperRole.UsesText);
                }

                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
                Coroutines.Start(Utils.FlashCoroutine(Color.red, 1f));
            }
            player.Data.Role.TeamType = RoleTeamTypes.Impostor;
            RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player2.nameText().color = Patches.Colors.Impostor;
                }
            }

            if (CustomGameOptions.TraitorCanAssassin) new Assassin(player);

            foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
            {
                var snitchRole = (Snitch)snitch;
                if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchRole.SnitchArrows.Add(player.PlayerId, arrow);
                }
                else if (snitchRole.Revealed && PlayerControl.LocalPlayer.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchRole.ImpArrows.Add(arrow);
                }
            }

            foreach (var haunter in Role.GetRoles(RoleEnum.Haunter))
            {
                var haunterRole = (Haunter)haunter;
                if (haunterRole.Revealed && PlayerControl.LocalPlayer.Is(RoleEnum.Traitor))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    haunterRole.ImpArrows.Add(arrow);
                }
            }
        }

        public static void CheckTraitorSpawn(PlayerControl deadPlayer)
        {
            if (CustomGameOptions.GameMode == GameMode.Cultist || CustomGameOptions.GameMode == GameMode.KillingOnly) return;
            if (!TraitorOn) return;
            foreach (var traitor in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Traitor)))
            {
                TraitorCanSpawn = false;
                return;
            }
            var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != deadPlayer).ToList();
            foreach (var player in alivePlayers)
            {
                if (player.Data.IsImpostor() || (player.Is(Faction.NeutralKilling) && CustomGameOptions.NeutralKillingStopsTraitor))
                {
                    TraitorCanSpawn = false;
                    return;
                }
            }
            if (alivePlayers.Count < CustomGameOptions.LatestSpawn)
            {
                TraitorCanSpawn = false;
                return;
            }

            alivePlayers = alivePlayers.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.IsLover() && !x.IsExeTarget()).ToList();
            if (alivePlayers.Count == 0)
            {
                TraitorCanSpawn = false;
                return;
            }
            TraitorCanSpawn = true;
        }

        public static void RemoveTasks(PlayerControl player)
        {
            foreach (var task in player.myTasks)
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;

                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();
                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        foreach (var console in UnityEngine.Object.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    normalPlayerTask.taskStep = 0;
                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;
                    if ((normalPlayerTask.TaskType == TaskTypes.EmptyGarbage || normalPlayerTask.TaskType == TaskTypes.EmptyChute)
                        && (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4))
                        normalPlayerTask.taskStep = 1;
                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
        }
        public static void ArsonistLastKiller(PlayerControl deadPlayer)
        {
            var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != deadPlayer).ToList();
            foreach (var player in alives)
            {
                if (player.Data.IsImpostor() || player.Is(Faction.NeutralKilling)) return;
            }
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            role.LastKiller = true;
            return;
        }
    }
}
