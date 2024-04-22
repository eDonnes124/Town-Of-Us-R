using HarmonyLib;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.Roles;
using UnityEngine;
using System;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.ImitatorMod;
using AmongUs.GameOptions;
using TownOfUs.Roles.Modifiers;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Patches.ScreenEffects;

namespace TownOfUs.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
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
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            Utils.Rpc(CustomRPC.Remember, PlayerControl.LocalPlayer.PlayerId, playerId);

            Remember(role, player);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            var amnesiac = amneRole.Player;

            var rememberImp = true;
            var rememberNeut = true;

            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = Role.GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
                foreach (var body in amnesiacRole.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            switch (role)
            {
                case RoleEnum.Sheriff:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Swapper:
                case RoleEnum.Investigator:
                case RoleEnum.Medic:
                case RoleEnum.Seer:
                case RoleEnum.Spy:
                case RoleEnum.Snitch:
                case RoleEnum.Altruist:
                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Hunter:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Mystic:
                case RoleEnum.Trapper:
                case RoleEnum.Detective:
                case RoleEnum.Imitator:
                case RoleEnum.VampireHunter:
                case RoleEnum.Prosecutor:
                case RoleEnum.Oracle:
                case RoleEnum.Aurial:

                    rememberImp = false;
                    rememberNeut = false;

                    break;

                case RoleEnum.Jester:
                case RoleEnum.Executioner:
                case RoleEnum.Arsonist:
                case RoleEnum.Amnesiac:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:
                case RoleEnum.Survivor:
                case RoleEnum.GuardianAngel:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Pestilence:
                case RoleEnum.Werewolf:
                case RoleEnum.Doomsayer:
                case RoleEnum.Vampire:

                    rememberImp = false;

                    break;
            }

            newRole = Role.GetRole(other);
            newRole.Player = amnesiac;

            if (role == RoleEnum.Aurial && PlayerControl.LocalPlayer == other)
            {
                var aurial = Role.GetRole<Aurial>(other);
                aurial.NormalVision = true;
                SeeAll.AllToNormal();
                CameraEffect.singleton.materials.Clear();
            }

            if ((role == RoleEnum.Glitch || role == RoleEnum.Juggernaut || role == RoleEnum.Pestilence ||
                role == RoleEnum.Werewolf) && PlayerControl.LocalPlayer == other)
            {
                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
            }

            if (role == RoleEnum.Investigator) Footprint.DestroyAll(Role.GetRole<Investigator>(other));

            if (role == RoleEnum.Snitch) CompleteTask.Postfix(amnesiac);

            Role.RoleDictionary.Remove(amnesiac.PlayerId);
            Role.RoleDictionary.Remove(other.PlayerId);
            Role.RoleDictionary.Add(amnesiac.PlayerId, newRole);

            if (!(amnesiac.Is(RoleEnum.Crewmate) || amnesiac.Is(RoleEnum.Impostor))) newRole.RegenTask();

            if (other == StartImitate.ImitatingPlayer)
            {
                StartImitate.ImitatingPlayer = amneRole.Player;
                newRole.AddToRoleHistory(RoleEnum.Imitator);
            }
            else newRole.AddToRoleHistory(newRole.RoleType);

            if (rememberImp == false)
            {
                if (rememberNeut == false)
                {
                    new Crewmate(other);
                }
                else
                {
                    // If role is not Vampire, turn dead player into Survivor
                    if (role != RoleEnum.Vampire)
                    {
                        var survivor = new Survivor(other);
                        survivor.RegenTask();
                    }
                    // If role is Vampire, keep dead player as Vampire
                    if (role == RoleEnum.Vampire)
                    {
                        var vampire = new Vampire(other);
                        vampire.RegenTask();
                    }

                    if (role == RoleEnum.Arsonist || role == RoleEnum.Glitch || role == RoleEnum.Plaguebearer ||
                            role == RoleEnum.Pestilence || role == RoleEnum.Werewolf || role == RoleEnum.Juggernaut
                             || role == RoleEnum.Vampire)
                    {
                        if (CustomGameOptions.AmneTurnNeutAssassin) new Assassin(amnesiac);
                        if (other.Is(AbilityEnum.Assassin)) Ability.AbilityDictionary.Remove(other.PlayerId);
                    }
                }
            }
            else if (rememberImp == true)
            {
                new Impostor(other);
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(amnesiac, RoleTypes.Impostor);
                amnesiac.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        player.nameText().color = Patches.Colors.Impostor;
                    }
                }
                if (CustomGameOptions.AmneTurnImpAssassin) new Assassin(amnesiac);
            }

            if (role == RoleEnum.Snitch)
            {
                var snitchRole = Role.GetRole<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);
                if (other.AmOwner)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
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
                engiRole.UsesLeft = CustomGameOptions.MaxFixes;
            }

            else if (role == RoleEnum.Medic)
            {
                var medicRole = Role.GetRole<Medic>(amnesiac);
                if (amnesiac != StartImitate.ImitatingPlayer) medicRole.UsedAbility = false;
                else medicRole.UsedAbility = true;
            }

            else if (role == RoleEnum.Mayor)
            {
                var mayorRole = Role.GetRole<Mayor>(amnesiac);
                mayorRole.Revealed = false;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Prosecutor)
            {
                var prosRole = Role.GetRole<Prosecutor>(amnesiac);
                prosRole.Prosecuted = false;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Vigilante)
            {
                var vigiRole = Role.GetRole<Vigilante>(amnesiac);
                vigiRole.RemainingKills = CustomGameOptions.VigilanteKills;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Veteran)
            {
                var vetRole = Role.GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Hunter)
            {
                var hunterRole = Role.GetRole<Hunter>(amnesiac);
                hunterRole.UsesLeft = CustomGameOptions.HunterStalkUses;
                hunterRole.LastStalked = DateTime.UtcNow;
                hunterRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Tracker)
            {
                var trackerRole = Role.GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                trackerRole.LastTracked = DateTime.UtcNow;
            }

            else if (role == RoleEnum.VampireHunter)
            {
                var vhRole = Role.GetRole<VampireHunter>(amnesiac);
                if (vhRole.AddedStakes) vhRole.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                else vhRole.UsesLeft = 0;
                vhRole.LastStaked = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Detective)
            {
                var detectiveRole = Role.GetRole<Detective>(amnesiac);
                detectiveRole.LastExamined = DateTime.UtcNow;
                detectiveRole.CurrentTarget = null;
                detectiveRole.LastKiller = null;
            }

            else if (role == RoleEnum.Mystic)
            {
                var mysticRole = Role.GetRole<Mystic>(amnesiac);
                mysticRole.BodyArrows.Values.DestroyAll();
                mysticRole.BodyArrows.Clear();
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Transporter)
            {
                var tpRole = Role.GetRole<Transporter>(amnesiac);
                tpRole.PressedButton = false;
                tpRole.MenuClick = false;
                tpRole.LastMouse = false;
                tpRole.TransportList = null;
                tpRole.TransportPlayer1 = null;
                tpRole.TransportPlayer2 = null;
                tpRole.LastTransported = DateTime.UtcNow;
                tpRole.UsesLeft = CustomGameOptions.TransportMaxUses;
            }

            else if (role == RoleEnum.Medium)
            {
                var medRole = Role.GetRole<Medium>(amnesiac);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
                medRole.LastMediated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Seer)
            {
                var seerRole = Role.GetRole<Seer>(amnesiac);
                seerRole.Investigated.RemoveRange(0, seerRole.Investigated.Count);
                seerRole.LastInvestigated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Oracle)
            {
                var oracleRole = Role.GetRole<Oracle>(amnesiac);
                oracleRole.Confessor = null;
                oracleRole.LastConfessed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Aurial)
            {
                var aurialRole = Role.GetRole<Aurial>(amnesiac);
                aurialRole.LastRadiated = DateTime.UtcNow;
                aurialRole.NormalVision = false;
                aurialRole.knownPlayerRoles.Clear();
                if (amnesiac.AmOwner) aurialRole.ApplyEffect();
                aurialRole.Loaded = true;
            }

            else if (role == RoleEnum.Arsonist)
            {
                var arsoRole = Role.GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Survivor)
            {
                var survRole = Role.GetRole<Survivor>(amnesiac);
                survRole.LastVested = DateTime.UtcNow;
                survRole.UsesLeft = CustomGameOptions.MaxVests;
            }

            else if (role == RoleEnum.GuardianAngel)
            {
                var gaRole = Role.GetRole<GuardianAngel>(amnesiac);
                gaRole.LastProtected = DateTime.UtcNow;
                gaRole.UsesLeft = CustomGameOptions.MaxProtects;
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

            else if (role == RoleEnum.Escapist)
            {
                var escapistRole = Role.GetRole<Escapist>(amnesiac);
                escapistRole.LastEscape = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Swooper)
            {
                var swooperRole = Role.GetRole<Swooper>(amnesiac);
                swooperRole.LastSwooped = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Venerer)
            {
                var venererRole = Role.GetRole<Venerer>(amnesiac);
                venererRole.LastCamouflaged = DateTime.UtcNow;
                venererRole.KillsAtStartAbility = 0;
            }

            else if (role == RoleEnum.Blackmailer)
            {
                var blackmailerRole = Role.GetRole<Blackmailer>(amnesiac);
                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
                blackmailerRole.Blackmailed = null;
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

            else if (role == RoleEnum.Werewolf)
            {
                var wwRole = Role.GetRole<Werewolf>(amnesiac);
                wwRole.LastRampaged = DateTime.UtcNow;
                wwRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Doomsayer)
            {
                var doomRole = Role.GetRole<Doomsayer>(amnesiac);
                doomRole.GuessedCorrectly = 0;
                doomRole.LastObserved = DateTime.UtcNow;
                doomRole.LastObservedPlayer = null;
            }

            else if (role == RoleEnum.Plaguebearer)
            {
                var plagueRole = Role.GetRole<Plaguebearer>(amnesiac);
                plagueRole.InfectedPlayers.RemoveRange(0, plagueRole.InfectedPlayers.Count);
                plagueRole.InfectedPlayers.Add(amnesiac.PlayerId);
                plagueRole.LastInfected = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Pestilence)
            {
                var pestRole = Role.GetRole<Pestilence>(amnesiac);
                pestRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Vampire)
            {
                var vampRole = Role.GetRole<Vampire>(amnesiac);
                vampRole.LastBit = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Trapper)
            {
                var trapperRole = Role.GetRole<Trapper>(amnesiac);
                trapperRole.LastTrapped = DateTime.UtcNow;
                trapperRole.UsesLeft = CustomGameOptions.MaxTraps;
                trapperRole.trappedPlayers.Clear();
                trapperRole.traps.ClearTraps();
            }

            else if (role == RoleEnum.Bomber)
            {
                var bomberRole = Role.GetRole<Bomber>(amnesiac);
                bomberRole.Bomb.ClearBomb();
            }

            else if (!(amnesiac.Is(RoleEnum.Altruist) || amnesiac.Is(RoleEnum.Amnesiac) || amnesiac.Is(Faction.Impostors)))
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            var killsList = (newRole.Kills, newRole.CorrectKills, newRole.IncorrectKills, newRole.CorrectAssassinKills, newRole.IncorrectAssassinKills);
            var otherRole = Role.GetRole(other);
            newRole.Kills = otherRole.Kills;
            newRole.CorrectKills = otherRole.CorrectKills;
            newRole.IncorrectKills = otherRole.IncorrectKills;
            newRole.CorrectAssassinKills = otherRole.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = otherRole.IncorrectAssassinKills;
            otherRole.Kills = killsList.Kills;
            otherRole.CorrectKills = killsList.CorrectKills;
            otherRole.IncorrectKills = killsList.IncorrectKills;
            otherRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            otherRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;

            if (amnesiac.Is(Faction.Impostors) && (!amnesiac.Is(RoleEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
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
        }
    }
}
