using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;

namespace TownOfUs.NeutralRoles.ShifterMod
{
    public enum ShiftEnum
    {
        NonImps,
        Crew
    }

    public enum BecomeEnum
    {
        Shifter,
        Crewmate
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    [HarmonyPriority(Priority.Last)]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Shifter);
            if (!flag) return true;
            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.ShifterShiftTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var playerId = role.ClosestPlayer.PlayerId;
            if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;

                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer1.Write(medic);
                writer1.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);
                if (CustomGameOptions.ShieldBreaks) role.LastShifted = DateTime.UtcNow;
                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Shift, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Shift(role, role.ClosestPlayer);
            return false;
        }

        public static async Task ShowShiftAsync()
        {
            var waitTime = TimeSpan.FromSeconds(0.83333336);
            var hud = DestroyableSingleton<HudManager>.Instance;
            var overlay = hud.KillOverlay;
            var transform = overlay.flameParent.transform;
            var flame = transform.GetChild(0).gameObject;
            var renderer = flame.GetComponent<SpriteRenderer>();

            renderer.sprite = TownOfUs.ShiftKill;
            overlay.flameParent.SetActive(true);

            // Skala w górę
            await LerpAsync(0.16666667f, t =>
            {
                overlay.flameParent.transform.localScale = new Vector3(1f, t, 1f);
            });

            // Czekaj 1 sekundę
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Skala w dół
            await LerpAsync(0.16666667f, t =>
            {
                overlay.flameParent.transform.localScale = new Vector3(1f, 1f - t, 1f);
            });

            overlay.flameParent.SetActive(false);
            overlay.showAll = null;
            renderer.sprite = TownOfUs.NormalKill;
        }

        private static async Task LerpAsync(float duration, Action<float> updateAction)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += UnityEngine.Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                updateAction(t);
                await Task.Yield(); // Umożliwia współbieżność
            }
        }




        public static async void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            //System.Console.WriteLine(role);
            //TODO - Shift Animation
            shifterRole.LastShifted = DateTime.UtcNow;
            var shifter = shifterRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<NetworkedPlayerInfo.TaskInfo> taskinfos1, taskinfos2;

            var swapTasks = true;
            var resetShifter = false;
            var snitch = false;
            var shift = false;

            Role newRole;

            switch (role)
            {
                case RoleEnum.Altruist:
                case RoleEnum.Aurial:
                case RoleEnum.Detective:
                case RoleEnum.Engineer:
                case RoleEnum.Haunter:
                case RoleEnum.Hunter:
                case RoleEnum.Imitator:
                case RoleEnum.Investigator:
                case RoleEnum.Jailor:
                case RoleEnum.Mayor:
                case RoleEnum.Medic:
                case RoleEnum.Medium:
                case RoleEnum.Mystic:
                case RoleEnum.Oracle:
                case RoleEnum.Politician:
                case RoleEnum.Prosecutor:
                case RoleEnum.President:
                case RoleEnum.Seer:
                case RoleEnum.Sheriff:
                case RoleEnum.Snitch:
                case RoleEnum.Spy:
                case RoleEnum.Swapper:
                case RoleEnum.TimeLord:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Trapper:
                case RoleEnum.Veteran:
                case RoleEnum.Vigilante:
                case RoleEnum.Warden:
                case RoleEnum.Crewmate:

                    shift = true;

                    break;
                case RoleEnum.Amnesiac:
                case RoleEnum.Arsonist:
                case RoleEnum.Doomsayer:
                case RoleEnum.Executioner:
                case RoleEnum.Glitch:
                case RoleEnum.GuardianAngel:
                case RoleEnum.Jester:
                case RoleEnum.Juggernaut:
                case RoleEnum.Phantom:
                case RoleEnum.Plaguebearer:
                case RoleEnum.SoulCollector:
                case RoleEnum.Survivor:
                case RoleEnum.Vampire:
                case RoleEnum.Werewolf:
                case RoleEnum.Shifter:
                    
                    if (CustomGameOptions.WhoShifts == ShiftEnum.NonImps)
                    {
                        shift = true;
                    }
                    break;
            }

            if (shift == true)
            {
                if (role == RoleEnum.Investigator) Footprint.DestroyAll(Role.GetRole<Investigator>(other));


                newRole = Role.GetRole(other);
                newRole.Player = shifter;

                if (role == RoleEnum.Snitch) CompleteTask.Postfix(shifter);

                var modifier = Modifier.GetModifier(other);
                var modifier2 = Modifier.GetModifier(shifter);

                if (modifier != null && modifier2 != null)
                {
                    modifier.Player = shifter;
                    modifier2.Player = other;
                    Modifier.ModifierDictionary.Remove(other.PlayerId);
                    Modifier.ModifierDictionary.Remove(shifter.PlayerId);
                    Modifier.ModifierDictionary.Add(shifter.PlayerId, modifier);
                    Modifier.ModifierDictionary.Add(other.PlayerId, modifier2);
                }
                else if (modifier2 != null)
                {
                    modifier2.Player = other;
                    Modifier.ModifierDictionary.Remove(shifter.PlayerId);
                    Modifier.ModifierDictionary.Add(other.PlayerId, modifier2);
                }
                else if (modifier != null)
                {
                    modifier.Player = shifter;
                    Modifier.ModifierDictionary.Remove(other.PlayerId);
                    Modifier.ModifierDictionary.Add(shifter.PlayerId, modifier);
                }


                Role.RoleDictionary.Remove(shifter.PlayerId);
                Role.RoleDictionary.Remove(other.PlayerId);
                Role.RoleDictionary.Add(shifter.PlayerId, newRole);

                snitch = role == RoleEnum.Snitch;

                foreach (var exeRole in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Executioner))
                {
                    var executioner = (Executioner)exeRole;
                    var target = executioner.target;
                    if (other == target)
                    {
                        executioner.target.nameText().color = Color.white;
                        
                        executioner.target = shifter;

                        executioner.RegenTask();
                    }
                }

                if (CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter)
                {
                    resetShifter = true;
                    shifterRole.Player = other;
                    Role.RoleDictionary.Add(other.PlayerId, shifterRole);
                }
                else
                {
                    new Crewmate(other);
                }
            }
            else
            {
                Utils.RpcMurderPlayer(shifter, shifter);
                swapTasks = false;
            }
            if (swapTasks)
            {
                tasks1 = other.myTasks;
                taskinfos1 = other.Data.Tasks;
                tasks2 = shifter.myTasks;
                taskinfos2 = shifter.Data.Tasks;

                shifter.myTasks = tasks1;
                shifter.Data.Tasks = taskinfos1;
                other.myTasks = tasks2;
                other.Data.Tasks = taskinfos2;

                if (other.AmOwner)
                {
                    await ShowShiftAsync();
                }

                if (snitch)
                {
                    var snitchRole = Role.GetRole<Snitch>(shifter);
                    snitchRole.ImpArrows.DestroyAll();
                    snitchRole.SnitchArrows.Clear();
                    CompleteTask.Postfix(shifter);
                    if (other.AmOwner)
                        foreach (var player in PlayerControl.AllPlayerControls)
                            player.nameText().color = Color.white;
                }

                if (resetShifter) shifterRole.RegenTask();
            }

            //System.Console.WriteLine(shifter.Is(RoleEnum.Sheriff));
            //System.Console.WriteLine(other.Is(RoleEnum.Sheriff));
            //System.Console.WriteLine(Roles.Role.GetRole(shifter));

            if (shifter.Is(RoleEnum.Arsonist))
            {
                var role2 = Role.GetRole<Arsonist>(shifter);
                if (role2.DousedPlayers.Contains(shifter.PlayerId))
                {
                    role2.DousedPlayers.Add(other.PlayerId);
                    role2.DousedPlayers.Remove(shifter.PlayerId);
                }
            }

            if (shifter.AmOwner || other.AmOwner)
            {
                if (shifter.Is(RoleEnum.Arsonist) && other.AmOwner)

                {
                    Role.GetRole<Arsonist>(shifter).IgniteButton.Destroy();
                }

                foreach (var sheriffRole in Role.GetRoles(RoleEnum.Sheriff))
                {
                    var sheriff = (Sheriff)sheriffRole;
                    sheriff.LastKilled = DateTime.UtcNow;
                    sheriff.LastKilled = sheriff.LastKilled.AddSeconds(2.5f - CustomGameOptions.SheriffKillCd);
                }

                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
             //   DestroyableSingleton<HudManager>.Instance.KillButton.isActive = false;
            }
        }
    }
}
