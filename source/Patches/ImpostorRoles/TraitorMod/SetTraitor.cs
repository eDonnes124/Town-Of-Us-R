using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.Extensions;
using UnityEngine;
using Reactor.Utilities;
using TownOfUs.Patches;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Modifiers;
using Il2CppSystem.Linq;
using TownOfUs.CrewmateRoles.PlumberMod;

namespace TownOfUs.ImpostorRoles.TraitorMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetTraitor.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetTraitor
    {
        public static PlayerControl WillBeTraitor;
        public static bool TraitorOn;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (!TraitorOn) return;
            if (!AmongUsClient.Instance.AmHost || WillBeTraitor != null) return;
            var exiled = __instance.initData.networkedPlayer?.Object;
            var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != exiled).ToList();
            foreach (var player in alives)
            {
                if (player.Data.IsImpostor() || (player.Is(Faction.NeutralKilling) && CustomGameOptions.NeutralKillingStopsTraitor))
                {
                    return;
                }
            }
            if (alives.Count < CustomGameOptions.LatestSpawn) return;

            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor) &&
                !x.Is(ModifierEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected && !x.IsExeTarget()).ToList();
            if (toChooseFrom.Count == 0) return;

            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];
            WillBeTraitor = pc;

            Utils.Rpc(CustomRPC.TraitorSpawn, WillBeTraitor.PlayerId);

            TurnImp();
        }

        public static void TurnImp()
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Traitor) && PlayerControl.LocalPlayer == WillBeTraitor)
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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                {
                    var detecRole = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                    detecRole.ExamineButton.gameObject.SetActive(false);
                    foreach (GameObject scene in detecRole.CrimeScenes)
                    {
                        UnityEngine.Object.Destroy(scene);
                    }
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Cleric))
                {
                    var clericRole = Role.GetRole<Cleric>(PlayerControl.LocalPlayer);
                    clericRole.CleanseButton.SetTarget(null);
                    clericRole.CleanseButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
                {
                    var oracleRole = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
                    oracleRole.BlessButton.SetTarget(null);
                    oracleRole.BlessButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                {
                    var hunterRole = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(hunterRole.UsesText);
                    hunterRole.StalkButton.SetTarget(null);
                    hunterRole.StalkButton.gameObject.SetActive(false);
                    HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                {
                    var engineerRole = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
                    Object.Destroy(engineerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Plumber))
                {
                    var plumberRole = Role.GetRole<Plumber>(PlayerControl.LocalPlayer);
                    plumberRole.Vent = null;
                    UnityEngine.Object.Destroy(plumberRole.UsesText);

                    try
                    {
                        HudManager.Instance.ImpostorVentButton.buttonLabelText.text = "VENT";
                        HudManager.Instance.ImpostorVentButton.graphic.sprite = VentButtonSprite.Vent;
                        HudManager.Instance.ImpostorVentButton.SetCoolDown(0f, 10f);
                        UnityEngine.Object.Destroy(HudManager.Instance.ImpostorVentButton.cooldownTimerText);
                        HudManager.Instance.ImpostorVentButton.gameObject.SetActive(false);
                    }
                    catch { }
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
                {
                    var aurialRole = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                    aurialRole.SenseArrows.Values.DestroyAll();
                    aurialRole.SenseArrows.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
                {
                    var loRole = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
                    Object.Destroy(loRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                {
                    var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                    Object.Destroy(transporterRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                    Object.Destroy(veteranRole.UsesText);
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
                    Object.Destroy(trapperRole.UsesText);
                }
            }

            if (WillBeTraitor.Is(RoleEnum.Warden))
            {
                var warden = Role.GetRole<Warden>(WillBeTraitor);
                if (warden.Fortified != null) ShowShield.ResetVisor(warden.Fortified, warden.Player);
            }

            if (WillBeTraitor.Is(RoleEnum.Medic))
            {
                var medic = Role.GetRole<Medic>(WillBeTraitor);
                if (medic.ShieldedPlayer != null) ShowShield.ResetVisor(medic.ShieldedPlayer, medic.Player);
            }

            if (WillBeTraitor.Is(RoleEnum.Cleric))
            {
                var cleric = Role.GetRole<Cleric>(WillBeTraitor);
                if (cleric.Barriered != null) cleric.UnBarrier();
            }

            if (WillBeTraitor.Is(RoleEnum.Plumber))
            {
                var plumberRole = Role.GetRole<Plumber>(WillBeTraitor);
                foreach (GameObject barricade in plumberRole.Barricades)
                {
                    UnityEngine.Object.Destroy(barricade);
                }
            }

            var oldRole = Role.GetRole(WillBeTraitor);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            Role.RoleDictionary.Remove(WillBeTraitor.PlayerId);
            var role = new Traitor(WillBeTraitor);
            if (StartImitate.ImitatingPlayers.Contains(WillBeTraitor.PlayerId))
            {
                role.formerRole = RoleEnum.Imitator;
                StartImitate.ImitatingPlayers.Remove(WillBeTraitor.PlayerId);
            }
            else role.formerRole = oldRole.RoleType;
            role.CorrectKills = killsList.CorrectKills;
            role.IncorrectKills = killsList.IncorrectKills;
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            if (PlayerControl.LocalPlayer == WillBeTraitor) role.RegenTask();

            WillBeTraitor.Data.Role.TeamType = RoleTeamTypes.Impostor;
            RoleManager.Instance.SetRole(WillBeTraitor, RoleTypes.Impostor);
            WillBeTraitor.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player2.nameText().color = Patches.Colors.Impostor;
                }
            }

            if (CustomGameOptions.TraitorCanAssassin) new Assassin(WillBeTraitor);

            if (PlayerControl.LocalPlayer.PlayerId == WillBeTraitor.PlayerId)
            {
                HudManager.Instance.KillButton.gameObject.SetActive(true);
                Coroutines.Start(Utils.FlashCoroutine(Color.red, 3f));
            }

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
                    snitchRole.SnitchArrows.Add(WillBeTraitor.PlayerId, arrow);
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

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Traitor))
            {
                if (CustomGameOptions.JanitorOn > 0) role.CanBeRoles.Add(RoleEnum.Janitor);
                if (CustomGameOptions.MorphlingOn > 0) role.CanBeRoles.Add(RoleEnum.Morphling);
                if (CustomGameOptions.MinerOn > 0) role.CanBeRoles.Add(RoleEnum.Miner);
                if (CustomGameOptions.SwooperOn > 0) role.CanBeRoles.Add(RoleEnum.Swooper);
                if (CustomGameOptions.UndertakerOn > 0) role.CanBeRoles.Add(RoleEnum.Undertaker);
                if (CustomGameOptions.EscapistOn > 0) role.CanBeRoles.Add(RoleEnum.Escapist);
                if (CustomGameOptions.GrenadierOn > 0) role.CanBeRoles.Add(RoleEnum.Grenadier);
                if (CustomGameOptions.BlackmailerOn > 0) role.CanBeRoles.Add(RoleEnum.Blackmailer);
                if (CustomGameOptions.BomberOn > 0) role.CanBeRoles.Add(RoleEnum.Bomber);
                if (CustomGameOptions.WarlockOn > 0) role.CanBeRoles.Add(RoleEnum.Warlock);
                if (CustomGameOptions.VenererOn > 0) role.CanBeRoles.Add(RoleEnum.Venerer);
                if (CustomGameOptions.HypnotistOn > 0) role.CanBeRoles.Add(RoleEnum.Hypnotist);
                if (CustomGameOptions.ScavengerOn > 0) role.CanBeRoles.Add(RoleEnum.Scavenger);
                if (CustomGameOptions.EclipsalOn > 0) role.CanBeRoles.Add(RoleEnum.Eclipsal);

                if (role.CanBeRoles.Count > 0)
                {
                    role.CanBeRoles.Shuffle();
                    while (role.CanBeRoles.Count > 3) role.CanBeRoles.RemoveAt(0);
                    var pk = new TraitorMenu((x) =>
                    {
                        RoleEnum selectedRole = x;
                        Utils.Rpc(CustomRPC.AddTraitorRole, PlayerControl.LocalPlayer.PlayerId, (byte)selectedRole);
                        ChangeRole(role, selectedRole);
                    });
                    Coroutines.Start(pk.Open(0f));
                }
            }
        }

        public static void ChangeRole(Traitor role, RoleEnum selectedRole)
        {
            var traitor = role.Player;
            var killsList = (role.Kills, role.CorrectKills, role.IncorrectKills, role.CorrectAssassinKills, role.IncorrectAssassinKills);
            var formerRole = role.formerRole;
            Role.RoleDictionary.Remove(traitor.PlayerId);
            if (selectedRole == RoleEnum.Janitor) new Janitor(traitor);
            else if (selectedRole == RoleEnum.Morphling) new Morphling(traitor);
            else if (selectedRole == RoleEnum.Miner) new Miner(traitor);
            else if (selectedRole == RoleEnum.Swooper) new Swooper(traitor);
            else if (selectedRole == RoleEnum.Undertaker) new Undertaker(traitor);
            else if (selectedRole == RoleEnum.Escapist) new Escapist(traitor);
            else if (selectedRole == RoleEnum.Grenadier) new Grenadier(traitor);
            else if (selectedRole == RoleEnum.Blackmailer) new Blackmailer(traitor);
            else if (selectedRole == RoleEnum.Bomber) new Bomber(traitor);
            else if (selectedRole == RoleEnum.Warlock) new Warlock(traitor);
            else if (selectedRole == RoleEnum.Venerer) new Venerer(traitor);
            else if (selectedRole == RoleEnum.Hypnotist) new Hypnotist(traitor);
            else if (selectedRole == RoleEnum.Scavenger)
            {
                new Scavenger(traitor);
                if (PlayerControl.LocalPlayer.killTimer <= 0f && PlayerControl.LocalPlayer == traitor) PlayerControl.LocalPlayer.SetKillTimer(0.01f);
            }
            else if (selectedRole == RoleEnum.Eclipsal) new Eclipsal(traitor);
            var newRole = Role.GetRole(traitor);
            newRole.Kills = killsList.Kills;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectKills = killsList.IncorrectKills;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            newRole.formerRole = formerRole;
            newRole.Name = "Traitor";
            newRole.RemoveFromRoleHistory(newRole.RoleType);
            if (PlayerControl.LocalPlayer == traitor) newRole.RegenTask();
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new System.Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}