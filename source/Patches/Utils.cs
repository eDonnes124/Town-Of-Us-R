using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using PerformKill = TownOfUs.Modifiers.UnderdogMod.PerformKill;
using Random = UnityEngine.Random;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.VampireHunterMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.AurialMod;
using Reactor.Networking;
using Reactor.Networking.Extensions;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;
        private static GameData.PlayerInfo voteTarget = null;

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision) return;
            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void Unmorph(PlayerControl player)
        {
            if (!(PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision)) player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision) return;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " ",
                        PetId = ""
                    });
                    PlayerMaterial.SetColors(Color.grey, player.myRend());
                    player.nameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;
                  
                }
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls) Unmorph(player);
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
            where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        public static bool IsLover(this PlayerControl player)
        {
            return player.Is(ModifierEnum.Lover);
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static bool Is(this PlayerControl player, AbilityEnum abilityType)
        {
            return Ability.GetAbility(player)?.AbilityType == abilityType;
        }

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)
            ).ToList();
        }

        public static List<PlayerControl> GetImpostors(
            List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();
            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null) return RoleEnum.None;
            if (player.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role != null) return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static bool IsExeTarget(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Executioner).Any(role =>
            {
                var exeTarget = ((Executioner)role).target;
                return exeTarget != null && player.PlayerId == exeTarget.PlayerId;
            });
        }

        public static bool IsShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static Medic GetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran != null && veteran.OnAlert && player.PlayerId == veteran.Player.PlayerId;
            });
        }

        public static bool IsVesting(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Survivor).Any(role =>
            {
                var surv = (Survivor)role;
                return surv != null && surv.Vesting && player.PlayerId == surv.Player.PlayerId;
            });
        }

        public static bool IsProtected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).target;
                var ga = (GuardianAngel)role;
                return gaTarget != null && ga.Protecting && player.PlayerId == gaTarget.PlayerId;
            });
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;
                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player.PlayerId == plaguebearer.Player.PlayerId);
            });
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false)
        {
            bool fullCooldownReset = false;
            bool gaReset = false;
            bool survReset = false;
            bool zeroSecReset = false;
            bool abilityUsed = false;
            if (target.IsInfected() || player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(target, player);
            }
            if (target == ShowRoundOneShield.FirstRoundShielded && toKill)
            {
                zeroSecReset = true;
            }
            else if (target.Is(RoleEnum.Pestilence))
            {
                if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                    else zeroSecReset = true;

                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected()) gaReset = true;
                else RpcMurderPlayer(target, player);
            }
            else if (target.IsOnAlert())
            {
                if (player.Is(RoleEnum.Pestilence)) zeroSecReset = true;
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                    else zeroSecReset = true;

                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected()) gaReset = true;
                else RpcMurderPlayer(target, player);
                if (toKill && CustomGameOptions.KilledOnAlert)
                {
                    if (target.IsShielded())
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        Rpc(CustomRPC.AttemptSound, medic, target.PlayerId);

                        if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                        else zeroSecReset = true;

                        StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected()) gaReset = true;
                    else
                    {
                        if (player.Is(RoleEnum.Glitch))
                        {
                            var glitch = Role.GetRole<Glitch>(player);
                            glitch.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Juggernaut))
                        {
                            var jugg = Role.GetRole<Juggernaut>(player);
                            jugg.JuggKills += 1;
                            jugg.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Pestilence))
                        {
                            var pest = Role.GetRole<Pestilence>(player);
                            pest.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Vampire))
                        {
                            var vamp = Role.GetRole<Vampire>(player);
                            vamp.LastBit = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.VampireHunter))
                        {
                            var vh = Role.GetRole<VampireHunter>(player);
                            vh.LastStaked = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Werewolf))
                        {
                            var ww = Role.GetRole<Werewolf>(player);
                            ww.LastKilled = DateTime.UtcNow;
                        }
                        RpcMurderPlayer(player, target);
                        abilityUsed = true;
                        fullCooldownReset = true;
                        gaReset = false;
                        zeroSecReset = false;
                    }
                }
            }
            else if (target.IsShielded() && toKill)
            {
                Rpc(CustomRPC.AttemptSound, target.GetMedic().Player.PlayerId, target.PlayerId);

                System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                else zeroSecReset = true;
                StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsVesting() && toKill)
            {
                survReset = true;
            }
            else if (target.IsProtected() && toKill)
            {
                gaReset = true;
            }
            else if (toKill)
            {
                if (player.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(player);
                    glitch.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Juggernaut))
                {
                    var jugg = Role.GetRole<Juggernaut>(player);
                    jugg.JuggKills += 1;
                    jugg.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Pestilence))
                {
                    var pest = Role.GetRole<Pestilence>(player);
                    pest.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Vampire))
                {
                    var vamp = Role.GetRole<Vampire>(player);
                    vamp.LastBit = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.VampireHunter))
                {
                    var vh = Role.GetRole<VampireHunter>(player);
                    vh.LastStaked = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Werewolf))
                {
                    var ww = Role.GetRole<Werewolf>(player);
                    ww.LastKilled = DateTime.UtcNow;
                }
                RpcMurderPlayer(player, target);
                abilityUsed = true;
                fullCooldownReset = true;
            }
            else
            {
                abilityUsed = true;
                fullCooldownReset = true;
            }
            var reset = new List<bool>();
            reset.Add(fullCooldownReset);
            reset.Add(gaReset);
            reset.Add(survReset);
            reset.Add(zeroSecReset);
            reset.Add(abilityUsed);
            return reset;
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead)
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            float lightRadius = radius * ShipStatus.Instance.MaxLightRadius;
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];
                if (!playerInfo.Disconnected && (!playerInfo.Object.Data.IsDead || includeDead))
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = ((Vector2)vector2).magnitude;
                    if (magnitude <= lightRadius)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }
            return playerControlList;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled || player.inVent) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }
            
            return result;
        }
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var player = GetClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, true);
            Rpc(CustomRPC.BypassKill, killer.PlayerId, target.PlayerId);
        }

        public static void RpcMultiMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, false);
            Rpc(CustomRPC.BypassMultiKill, killer.PlayerId, target.PlayerId);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool jumpToBody)
        {
            var data = target.Data;
            if (data != null && !data.IsDead)
            {
                if (ShowRoundOneShield.DiedFirst == "") ShowRoundOneShield.DiedFirst = target.GetDefaultOutfit().PlayerName;

                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                if (!killer.Is(Faction.Crewmates) && killer != target
                    && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) Role.GetRole(killer).Kills += 1;

                if (killer.Is(RoleEnum.Sheriff))
                {
                    var sheriff = Role.GetRole<Sheriff>(killer);
                    if (target.Is(Faction.Impostors) ||
                        target.Is(RoleEnum.Glitch) && CustomGameOptions.SheriffKillsGlitch ||
                        target.Is(RoleEnum.Arsonist) && CustomGameOptions.SheriffKillsArsonist ||
                        target.Is(RoleEnum.Plaguebearer) && CustomGameOptions.SheriffKillsPlaguebearer ||
                        target.Is(RoleEnum.Pestilence) && CustomGameOptions.SheriffKillsPlaguebearer ||
                        target.Is(RoleEnum.Werewolf) && CustomGameOptions.SheriffKillsWerewolf ||
                        target.Is(RoleEnum.Juggernaut) && CustomGameOptions.SheriffKillsJuggernaut ||
                        target.Is(RoleEnum.Vampire) && CustomGameOptions.SheriffKillsVampire ||
                        target.Is(RoleEnum.Executioner) && CustomGameOptions.SheriffKillsExecutioner ||
                        target.Is(RoleEnum.Doomsayer) && CustomGameOptions.SheriffKillsDoomsayer ||
                        target.Is(RoleEnum.Jester) && CustomGameOptions.SheriffKillsJester) sheriff.CorrectKills += 1;
                    else if (killer == target) sheriff.IncorrectKills += 1;
                }

                if (killer.Is(RoleEnum.VampireHunter))
                {
                    var vh = Role.GetRole<VampireHunter>(killer);
                    if (killer != target) vh.CorrectKills += 1;
                }

                if (killer.Is(RoleEnum.Veteran))
                {
                    var veteran = Role.GetRole<Veteran>(killer);
                    if (target.Is(Faction.Impostors) || target.Is(Faction.NeutralKilling) || target.Is(Faction.NeutralEvil)) veteran.CorrectKills += 1;
                    else if (killer != target) veteran.IncorrectKills += 1;
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.Mystic));
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                {
                    var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                    detective.LastKiller = killer;
                }

                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                if (jumpToBody)
                {
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                }
                else killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));

                if (target.Is(ModifierEnum.Frosty))
                {
                    var frosty = Modifier.GetModifier<Frosty>(target);
                    frosty.Chilled = killer;
                    frosty.LastChilled = DateTime.UtcNow;
                    frosty.IsChilled = true;
                }

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);

                if (MeetingHud.Instance) target.Exiled();

                if (!killer.AmOwner) return;

                if (target.Is(ModifierEnum.Bait))
                {
                    BaitReport(killer, target);
                }

                if (target.Is(ModifierEnum.Aftermath))
                {
                    Aftermath.ForceAbility(killer, target);
                }

                if (!jumpToBody) return;

                if (killer.Data.IsImpostor() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentHideNSeekGameOptions.KillCooldown);
                    return;
                }

                if (killer == PlayerControl.LocalPlayer && killer.Is(RoleEnum.Warlock))
                {
                    var warlock = Role.GetRole<Warlock>(killer);
                    if (warlock.Charging)
                    {
                        warlock.UsingCharge = true;
                        warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                        if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                    }
                    killer.SetKillTimer(0.01f);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var werewolf = Role.GetRole<Werewolf>(killer);
                    werewolf.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.RampageKillCd);
                    werewolf.Player.SetKillTimer(CustomGameOptions.RampageKillCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Vampire))
                {
                    var vampire = Role.GetRole<Vampire>(killer);
                    vampire.LastBit = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.BiteCd);
                    vampire.Player.SetKillTimer(CustomGameOptions.BiteCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = Role.GetRole<Juggernaut>(killer);
                    juggernaut.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * juggernaut.JuggKills));
                    juggernaut.Player.SetKillTimer((CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * juggernaut.JuggKills) * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier;
                    var upperKC = (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    killer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    killer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    return;
                }
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target)
        {
            Coroutines.Start(BaitReportDelay(killer, target));
        }

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            var extraDelay = Random.RandomRangeInt(0, (int) (100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay) + 1));
            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + extraDelay/100f);
            var bodies = Object.FindObjectsOfType<DeadBody>();
            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId) { killer.ReportDeadBody(target.Data); break; }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId)
                        {
                            Rpc(CustomRPC.BaitReport, killer.PlayerId, target.PlayerId);
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void Convert(PlayerControl player)
        {
            if (PlayerControl.LocalPlayer == player) Coroutines.Start(FlashCoroutine(Patches.Colors.Impostor));
            if (PlayerControl.LocalPlayer != player && PlayerControl.LocalPlayer.Is(RoleEnum.CultistMystic)
                && !PlayerControl.LocalPlayer.Data.IsDead) Coroutines.Start(FlashCoroutine(Patches.Colors.Impostor));

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter) && PlayerControl.LocalPlayer == player)
            {
                var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                Object.Destroy(transporterRole.UsesText);
                if (transporterRole.TransportList != null)
                {
                    transporterRole.TransportList.Toggle();
                    transporterRole.TransportList.SetVisible(false);
                    transporterRole.TransportList = null;
                    transporterRole.PressedButton = false;
                    transporterRole.TransportPlayer1 = null;
                }
            }

            if (player.Is(RoleEnum.Chameleon))
            {
                var chameleonRole = Role.GetRole<Chameleon>(player);
                if (chameleonRole.IsSwooped) chameleonRole.UnSwoop();
                Role.RoleDictionary.Remove(player.PlayerId);
                var swooper = new Swooper(player);
                swooper.LastSwooped = DateTime.UtcNow;
                swooper.RegenTask();
            }

            if (player.Is(RoleEnum.Engineer))
            {
                var engineer = Role.GetRole<Engineer>(player);
                engineer.Name = "Demolitionist";
                engineer.Color = Patches.Colors.Impostor;
                engineer.Faction = Faction.Impostors;
                engineer.RegenTask();
            }

            if (player.Is(RoleEnum.Investigator))
            {
                var investigator = Role.GetRole<Investigator>(player);
                investigator.Name = "Consigliere";
                investigator.Color = Patches.Colors.Impostor;
                investigator.Faction = Faction.Impostors;
                investigator.RegenTask();
            }

            if (player.Is(RoleEnum.CultistMystic))
            {
                var mystic = Role.GetRole<CultistMystic>(player);
                mystic.Name = "Clairvoyant";
                mystic.Color = Patches.Colors.Impostor;
                mystic.Faction = Faction.Impostors;
                mystic.RegenTask();
            }

            if (player.Is(RoleEnum.CultistSnitch))
            {
                var snitch = Role.GetRole<CultistSnitch>(player);
                snitch.Name = "Informant";
                snitch.TaskText = () => "Complete all your tasks to reveal a fake Impostor!";
                snitch.Color = Patches.Colors.Impostor;
                snitch.Faction = Faction.Impostors;
                snitch.RegenTask();
                if (PlayerControl.LocalPlayer == player && snitch.CompletedTasks)
                {
                    var crew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor)).ToList();
                    if (crew.Count != 0)
                    {
                        crew.Shuffle();
                        snitch.RevealedPlayer = crew[0];
                        Rpc(CustomRPC.SnitchCultistReveal, player.PlayerId, snitch.RevealedPlayer.PlayerId);
                    }
                }
            }

            if (player.Is(RoleEnum.Spy))
            {
                var spy = Role.GetRole<Spy>(player);
                spy.Name = "Rogue Agent";
                spy.Color = Patches.Colors.Impostor;
                spy.Faction = Faction.Impostors;
                spy.RegenTask();
            }

            if (player.Is(RoleEnum.Transporter))
            {
                Role.RoleDictionary.Remove(player.PlayerId);
                var escapist = new Escapist(player);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.RegenTask();
            }

            if (player.Is(RoleEnum.Vigilante))
            {
                var vigi = Role.GetRole<Vigilante>(player);
                vigi.Name = "Assassin";
                vigi.TaskText = () => "Guess the roles of crewmates mid-meeting to kill them!";
                vigi.Color = Patches.Colors.Impostor;
                vigi.Faction = Faction.Impostors;
                vigi.RegenTask();
                var colorMapping = new Dictionary<string, Color>();
                if (CustomGameOptions.MayorCultistOn > 0) colorMapping.Add("Mayor", Colors.Mayor);
                if (CustomGameOptions.SeerCultistOn > 0) colorMapping.Add("Seer", Colors.Seer);
                if (CustomGameOptions.SheriffCultistOn > 0) colorMapping.Add("Sheriff", Colors.Sheriff);
                if (CustomGameOptions.SurvivorCultistOn > 0) colorMapping.Add("Survivor", Colors.Survivor);
                if (CustomGameOptions.MaxChameleons > 0) colorMapping.Add("Chameleon", Colors.Chameleon);
                if (CustomGameOptions.MaxEngineers > 0) colorMapping.Add("Engineer", Colors.Engineer);
                if (CustomGameOptions.MaxInvestigators > 0) colorMapping.Add("Investigator", Colors.Investigator);
                if (CustomGameOptions.MaxMystics > 0) colorMapping.Add("Mystic", Colors.Mystic);
                if (CustomGameOptions.MaxSnitches > 0) colorMapping.Add("Snitch", Colors.Snitch);
                if (CustomGameOptions.MaxSpies > 0) colorMapping.Add("Spy", Colors.Spy);
                if (CustomGameOptions.MaxTransporters > 0) colorMapping.Add("Transporter", Colors.Transporter);
                if (CustomGameOptions.MaxVigilantes > 1) colorMapping.Add("Vigilante", Colors.Vigilante);
                colorMapping.Add("Crewmate", Colors.Crewmate);
                vigi.SortedColorMapping = colorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }

            if (player.Is(RoleEnum.Crewmate))
            {
                Role.RoleDictionary.Remove(player.PlayerId);
                new Impostor(player);
            }

            player.Data.Role.TeamType = RoleTeamTypes.Impostor;
            RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.CultistSnitch))
            {
                var snitch = Role.GetRole<CultistSnitch>(PlayerControl.LocalPlayer);
                if (snitch.RevealedPlayer == player)
                {
                    var crew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor)).ToList();
                    if (crew.Count != 0)
                    {
                        crew.Shuffle();
                        snitch.RevealedPlayer = crew[0];
                        Rpc(CustomRPC.SnitchCultistReveal, player.PlayerId, snitch.RevealedPlayer.PlayerId);
                    }
                }
            }

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player2.nameText().color = Patches.Colors.Impostor;
                }
            }
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {
            color.a = alpha;
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                if (fullscreen.color.Equals(color))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                    fullscreen.enabled = false;
                }
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void RemoveTasks(PlayerControl player)
        {
            var totalTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks + GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks +
                             GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;


            foreach (var task in player.myTasks)
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;

                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();
                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
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
                        normalPlayerTask.UpdateArrowAndLocation();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            GameManager.Instance.RpcEndGame(reason, showAds);
        }


        public static void Rpc(params object[] data)
        {
            if (data[0] is not CustomRPC) throw new ArgumentException($"first parameter must be a {typeof(CustomRPC).FullName}");

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)(CustomRPC)data[0], SendOption.Reliable, -1);

            if (data.Length == 1)
            {
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return;
            }

            foreach (var item in data[1..])
            {

                if (item is bool boolean)
                {
                    writer.Write(boolean);
                }
                else if (item is int integer)
                {
                    writer.Write(integer);
                }
                else if (item is uint uinteger)
                {
                    writer.Write(uinteger);
                }
                else if (item is float Float)
                {
                    writer.Write(Float);
                }
                else if (item is byte Byte)
                {
                    writer.Write(Byte);
                }
                else if (item is sbyte sByte)
                {
                    writer.Write(sByte);
                }
                else if (item is Vector2 vector)
                {
                    writer.Write(vector);
                }
                else if (item is Vector3 vector3)
                {
                    writer.Write(vector3);
                }
                else if (item is string str)
                {
                    writer.Write(str);
                }
                else if (item is byte[] array)
                {
                    writer.WriteBytesAndSize(array);
                }
                else
                {
                    Logger<TownOfUs>.Error($"unknown data type entered for rpc write: item - {nameof(item)}, {item.GetType().FullName}, rpc - {data[0]}");
                }
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        class MedScanMinigameFixedUpdatePatch
        {
            static void Prefix(MedScanMinigame __instance)
            {
                if (CustomGameOptions.ParallelMedScans)
                {
                    //Allows multiple medbay scans at once
                    __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                    __instance.medscan.UsersList.Clear();
                }
            }
        }
      
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget) {
                voteTarget = meetingTarget;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch {
            static void Postfix(MeetingHud __instance) {
                // Deactivate skip Button if skipping on emergency meetings is disabled 
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)) {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                }
            }
        }

        //Submerged utils
        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }
        public static IList createList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static void ResetCustomTimers()
        {
            #region CrewmateRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var medium = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                medium.LastMediated = DateTime.UtcNow;
            }
            foreach (var role in Role.GetRoles(RoleEnum.Medium))
            {
                var medium = (Medium)role;
                medium.MediatedPlayers.Values.DestroyAll();
                medium.MediatedPlayers.Clear();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                var seer = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                var oracle = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
                oracle.LastConfessed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
            {
                var aurial = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                aurial.LastRadiated = DateTime.UtcNow;
                aurial.CannotSeeDelay = DateTime.UtcNow;
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    aurial.NormalVision = true;
                    SeeAll.AllToNormal();
                    aurial.ClearEffect();
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer))
            {
                var seer = Role.GetRole<CultistSeer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
                sheriff.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                var tracker = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                tracker.LastTracked = DateTime.UtcNow;
                tracker.UsesLeft = CustomGameOptions.MaxTracks;
                if (CustomGameOptions.ResetOnNewRound)
                {
                    tracker.TrackerArrows.Values.DestroyAll();
                    tracker.TrackerArrows.Clear();
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
            {
                var vh = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
                vh.LastStaked = DateTime.UtcNow;
            }
            foreach (var vh in Role.GetRoles(RoleEnum.VampireHunter))
            {
                var vhRole = (VampireHunter)vh;
                if (!vhRole.AddedStakes)
                {
                    vhRole.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                    vhRole.AddedStakes = true;
                }
                var vamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (vamps.Count == 0 && vh.Player != StartImitate.ImitatingPlayer && !vh.Player.Data.IsDead && !vh.Player.Data.Disconnected)
                {
                    var vhPlayer = vhRole.Player;

                    if (CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Sheriff)
                    {
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var sheriff = new Sheriff(vhPlayer);
                        sheriff.CorrectKills = kills;
                        sheriff.RegenTask();
                    }
                    else if (CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Veteran)
                    {
                        if (PlayerControl.LocalPlayer == vhPlayer) Object.Destroy(((VampireHunter)vh).UsesText);
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var vet = new Veteran(vhPlayer);
                        vet.CorrectKills = kills;
                        vet.RegenTask();
                        vet.LastAlerted = DateTime.UtcNow;
                    }
                    else if (CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Vigilante)
                    {
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var vigi = new Vigilante(vhPlayer);
                        vigi.CorrectKills = kills;
                        vigi.RegenTask();
                    }
                    else
                    {
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var crew = new Crewmate(vhPlayer);
                        crew.CorrectKills = kills;
                    }
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                var transporter = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                transporter.LastTransported = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                var veteran = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                veteran.LastAlerted = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                var trapper = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                trapper.LastTrapped = DateTime.UtcNow;
                trapper.trappedPlayers.Clear();
                if (CustomGameOptions.TrapsRemoveOnNewRound) trapper.traps.ClearTraps();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                detective.LastExamined = DateTime.UtcNow;
                detective.ClosestPlayer = null;
                detective.CurrentTarget = null;
                detective.LastKiller = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                var chameleon = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);
                chameleon.LastSwooped = DateTime.UtcNow;
            }
            #endregion
            #region NeutralRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                var surv = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);
                surv.LastVested = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                var vamp = Role.GetRole<Vampire>(PlayerControl.LocalPlayer);
                vamp.LastBit = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                ga.LastProtected = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                var arsonist = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
                arsonist.LastDoused = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
            {
                var glitch = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastMimic = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                var juggernaut = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);
                juggernaut.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                var werewolf = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                werewolf.LastRampaged = DateTime.UtcNow;
                werewolf.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                var plaguebearer = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
                plaguebearer.LastInfected = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
            {
                var pest = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);
                pest.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doom = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                doom.LastObserved = DateTime.UtcNow;
                doom.LastObservedPlayer = null;
            }
            #endregion
            #region ImposterRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Escapist))
            {
                var escapist = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.EscapeButton.graphic.sprite = TownOfUs.MarkSprite;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer))
            {
                var blackmailer = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                if (blackmailer.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var blackmailer = (Blackmailer)role;
                blackmailer.Blackmailed = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
            {
                var bomber = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
                bomber.PlantButton.graphic.sprite = TownOfUs.PlantSprite;
                bomber.Bomb.ClearBomb();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier))
            {
                var grenadier = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);
                grenadier.LastFlashed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Miner))
            {
                var miner = Role.GetRole<Miner>(PlayerControl.LocalPlayer);
                miner.LastMined = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling))
            {
                var morphling = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.MorphButton.graphic.sprite = TownOfUs.SampleSprite;
                morphling.SampledPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
            {
                var swooper = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
                swooper.LastSwooped = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Venerer))
            {
                var venerer = Role.GetRole<Venerer>(PlayerControl.LocalPlayer);
                venerer.LastCamouflaged = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
            {
                var undertaker = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.DragDropButton.graphic.sprite = TownOfUs.DragSprite;
                undertaker.CurrentlyDragging = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer))
            {
                var necro = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);
                necro.LastRevived = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer))
            {
                var whisperer = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);
                whisperer.LastWhispered = DateTime.UtcNow;
            }
            #endregion
        }
    }
}
