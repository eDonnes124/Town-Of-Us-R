using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.Roles;
using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;
using Object = UnityEngine.Object;
using TownOfUs.Extensions;

namespace TownOfUs.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    [HarmonyPriority(Priority.Last)]
    public class PerformKillButton

    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Remember, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Remember(role, player);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            var amnesiac = amneRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;

            var rememberImp = true;
            var rememberNeut = true;

            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = Role.GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
            }

            switch (role)
            {
                case RoleEnum.Sheriff:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Swapper:
                case RoleEnum.Investigator:
                case RoleEnum.TimeLord:
                case RoleEnum.Medic:
                case RoleEnum.Seer:
                case RoleEnum.Spy:
                case RoleEnum.Snitch:
                case RoleEnum.Altruist:
                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Haunter:
                case RoleEnum.Phantom:

                    rememberImp = false;
                    rememberNeut = false;

                    break;


                case RoleEnum.Jester:
                case RoleEnum.Executioner:
                case RoleEnum.Arsonist:
                case RoleEnum.Amnesiac:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:

                    rememberImp = false;

                    break;
            }

            if (role == RoleEnum.Investigator) Footprint.DestroyAll(Role.GetRole<Investigator>(other));

            newRole = Role.GetRole(other);
            newRole.Player = amnesiac;

            if (role == RoleEnum.Snitch) CompleteTask.Postfix(amnesiac);

            Role.RoleDictionary.Remove(amnesiac.PlayerId);
            if (!(role == RoleEnum.Haunter || role == RoleEnum.Phantom))
            {
                Role.RoleDictionary.Remove(other.PlayerId);
                Role.RoleDictionary.Add(amnesiac.PlayerId, newRole);
                newRole.AddToRoleHistory(newRole.RoleType);
            }
            else
            {
                new Crewmate(amnesiac);
            }

            if (rememberImp == false && (!(role == RoleEnum.Haunter || role == RoleEnum.Phantom)))
            {
                if (rememberNeut == false)
                {
                    new Crewmate(other);
                }
                else
                {
                    new Jester(other);
                }
            }
            else if (rememberImp == true)
            {
                new Impostor(other);
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(amnesiac, RoleTypes.Impostor);
                amnesiac.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        player.nameText.color = Patches.Colors.Impostor;
                    }
                }
                if (CustomGameOptions.AmneTurnAssassin)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetAssassin, SendOption.Reliable, -1);
                    writer.Write(amnesiac.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                if (amnesiac.Is(RoleEnum.Poisoner))
                {
                    if (PlayerControl.LocalPlayer == amnesiac)
                    {
                        var poisonerRole = Role.GetRole<Poisoner>(amnesiac);
                        poisonerRole.LastPoisoned = DateTime.UtcNow;
                        DestroyableSingleton<HudManager>.Instance.KillButton.graphic.enabled = false;
                    }
                    else if (PlayerControl.LocalPlayer == other)
                    {
                        DestroyableSingleton<HudManager>.Instance.KillButton.enabled = true;
                        DestroyableSingleton<HudManager>.Instance.KillButton.graphic.enabled = true;
                    }
                }
            }

            tasks1 = other.myTasks;
            taskinfos1 = other.Data.Tasks;
            tasks2 = amnesiac.myTasks;
            taskinfos2 = amnesiac.Data.Tasks;

            amnesiac.myTasks = tasks1;
            amnesiac.Data.Tasks = taskinfos1;
            other.myTasks = tasks2;
            other.Data.Tasks = taskinfos2;

            if (role == RoleEnum.Snitch)
            {
                var snitchRole = Role.GetRole<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);
                if (other.AmOwner)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText.color = Color.white;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Sheriff)
            {
                var sheriffRole = Role.GetRole<Sheriff>(amnesiac);
                sheriffRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Engineer)
            {
                var engiRole = Role.GetRole<Engineer>(amnesiac);
                engiRole.UsedThisRound = false;
            }

            else if (role == RoleEnum.Medic)
            {
                var medicRole = Role.GetRole<Medic>(amnesiac);
                medicRole.UsedAbility = false;
            }

            else if (role == RoleEnum.Mayor)
            {
                var mayorRole = Role.GetRole<Mayor>(amnesiac);
                mayorRole.VoteBank = CustomGameOptions.MayorVoteBank;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Veteran)
            {
                var vetRole = Role.GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Tracker)
            {
                var trackerRole = Role.GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                trackerRole.LastTracked = DateTime.UtcNow;
            }

            else if (role == RoleEnum.TimeLord)
            {
                var tlRole = Role.GetRole<TimeLord>(amnesiac);
                tlRole.FinishRewind = DateTime.UtcNow;
                tlRole.StartRewind = DateTime.UtcNow;
                tlRole.StartRewind = tlRole.StartRewind.AddSeconds(-10.0f);
                tlRole.UsesLeft = CustomGameOptions.RewindMaxUses;
            }

            else if (role == RoleEnum.Transporter)
            {
                var tpRole = Role.GetRole<Transporter>(amnesiac);
                tpRole.PressedButton = false;
                tpRole.MenuClick = false;
                tpRole.LastMouse = false;
                tpRole.TransportList1 = null;
                tpRole.TransportList2 = null;
                tpRole.TransportPlayer1 = null;
                tpRole.TransportPlayer2 = null;
                tpRole.LastTransported = DateTime.UtcNow;
                tpRole.UsesLeft = CustomGameOptions.TransportMaxUses;
                
            }

            else if (role == RoleEnum.Seer)
            {
                var seerRole = Role.GetRole<Seer>(amnesiac);
                seerRole.Investigated.RemoveRange(0, seerRole.Investigated.Count);
                seerRole.LastInvestigated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Arsonist)
            {
                var arsoRole = Role.GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Glitch)
            {
                var glitchRole = Role.GetRole<Glitch>(amnesiac);
                glitchRole.LastKill = DateTime.UtcNow;
                glitchRole.LastHack = DateTime.UtcNow;
                glitchRole.LastMimic = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Juggernaut)
            {
                var juggRole = Role.GetRole<Juggernaut>(amnesiac);
                juggRole.JuggKills = 0;
                juggRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Camouflager)
            {
                var camoRole = Role.GetRole<Camouflager>(amnesiac);
                camoRole.LastCamouflaged = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Grenadier)
            {
                var grenadeRole = Role.GetRole<Grenadier>(amnesiac);
                grenadeRole.LastFlashed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Morphling)
            {
                var morphlingRole = Role.GetRole<Morphling>(amnesiac);
                morphlingRole.LastMorphed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Swooper)
            {
                var swooperRole = Role.GetRole<Swooper>(amnesiac);
                swooperRole.LastSwooped = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Miner)
            {
                var minerRole = Role.GetRole<Miner>(amnesiac);
                minerRole.LastMined = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }

            else if (!(amnesiac.Is(RoleEnum.Altruist) || amnesiac.Is(RoleEnum.Amnesiac) || amnesiac.Is(Faction.Impostors)))
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            if (amnesiac.Is(Faction.Impostors))
            {
                foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;
                    if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (snitchRole.Revealed && PlayerControl.LocalPlayer == amnesiac)
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
            }

            if (other.Is(RoleEnum.Crewmate))
            {
                var role2 = Role.GetRole<Crewmate>(other);
                role2.RegenTask();
            }
            else if (other.Is(RoleEnum.Jester))
            {
                var role2 = Role.GetRole<Jester>(other);
                role2.RegenTask();
            }
            else
            {
                var role2 = Role.GetRole<Impostor>(other);
                role2.RegenTask();
            }
            
            Lights.SetLights();
        }
    }
}
