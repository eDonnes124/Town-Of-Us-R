using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.Patches.ScreenEffects;
using System;

namespace TownOfUs.Patches {

    static class AdditionalTempData {
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();
        public static List<Winners> otherWinners = new List<Winners>();

        public static void clear() {
            playerRoles.Clear();
            otherWinners.Clear();
        }

        internal class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public string Role { get; set; }
        }

        internal class Winners
        {
            public string PlayerName { get; set; }
            public RoleEnum Role { get; set; }
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch {

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            if (CameraEffect.singleton) CameraEffect.singleton.materials.Clear();
            AdditionalTempData.clear();
            var playerRole = "";
            var temporaryRoleName = "";
            var roleColor = "";
            // Theres a better way of doing this e.g. switch statement or dictionary. But this works for now.
            foreach (var playerControl in PlayerControl.AllPlayerControls)
            {
                playerRole = "";
                temporaryRoleName = "";
                foreach (var role in Role.RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    /*temporaryRoleName = role.Value.ToString();
                    var roleColorObject = typeof(Colors).GetProperty(role.Value.ToString())?.GetValue(null) as dynamic;
                    if (roleColorObject != null) {
                        roleColor = roleColorObject.ToHtmlStringRGBA();
                        playerRole += "<color=#" + roleColor + ">" + temporaryRoleName + "</color> > ";
                    } else {
                        playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">" + temporaryRoleName + "</color> > ";
                    }*/
                    if (role.Value == RoleEnum.Crewmate) { playerRole += "<color=#" + Patches.Colors.Crewmate.ToHtmlStringRGBA() + ">Crewmate</color> > "; }
                    else if (role.Value == RoleEnum.Impostor) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Impostor</color> > "; }
                    else if (role.Value == RoleEnum.Altruist) { playerRole += "<color=#" + Patches.Colors.Altruist.ToHtmlStringRGBA() + ">Altruist</color> > "; }
                    else if (role.Value == RoleEnum.Engineer) { playerRole += "<color=#" + Patches.Colors.Engineer.ToHtmlStringRGBA() + ">Engineer</color> > "; }
                    else if (role.Value == RoleEnum.Investigator) { playerRole += "<color=#" + Patches.Colors.Investigator.ToHtmlStringRGBA() + ">Investigator</color> > "; }
                    else if (role.Value == RoleEnum.TimeLord) { playerRole += "<color=#" + Patches.Colors.TimeLord.ToHtmlStringRGBA() + ">Time Lord</color> > "; }
                    else if (role.Value == RoleEnum.President) { playerRole += "<color=#" + Patches.Colors.President.ToHtmlStringRGBA() + ">President</color> > "; }
                    else if (role.Value == RoleEnum.Mayor) { playerRole += "<color=#" + Patches.Colors.Mayor.ToHtmlStringRGBA() + ">Mayor</color> > "; }
                    else if (role.Value == RoleEnum.Medic) { playerRole += "<color=#" + Patches.Colors.Medic.ToHtmlStringRGBA() + ">Medic</color> > "; }
                    else if (role.Value == RoleEnum.Sheriff) { playerRole += "<color=#" + Patches.Colors.Sheriff.ToHtmlStringRGBA() + ">Sheriff</color> > "; }
                    else if (role.Value == RoleEnum.Swapper) { playerRole += "<color=#" + Patches.Colors.Swapper.ToHtmlStringRGBA() + ">Swapper</color> > "; }
                    else if (role.Value == RoleEnum.Seer) { playerRole += "<color=#" + Patches.Colors.Seer.ToHtmlStringRGBA() + ">Seer</color> > "; }
                    else if (role.Value == RoleEnum.Snitch) { playerRole += "<color=#" + Patches.Colors.Snitch.ToHtmlStringRGBA() + ">Snitch</color> > "; }
                    else if (role.Value == RoleEnum.Spy) { playerRole += "<color=#" + Patches.Colors.Spy.ToHtmlStringRGBA() + ">Spy</color> > "; }
                    else if (role.Value == RoleEnum.Vigilante) { playerRole += "<color=#" + Patches.Colors.Vigilante.ToHtmlStringRGBA() + ">Vigilante</color> > "; }
                    else if (role.Value == RoleEnum.Hunter) { playerRole += "<color=#" + Patches.Colors.Hunter.ToHtmlStringRGBA() + ">Hunter</color> > "; }
                    else if (role.Value == RoleEnum.Arsonist) { playerRole += "<color=#" + Patches.Colors.Arsonist.ToHtmlStringRGBA() + ">Arsonist</color> > "; }
                    else if (role.Value == RoleEnum.Executioner) { playerRole += "<color=#" + Patches.Colors.Executioner.ToHtmlStringRGBA() + ">Executioner</color> > "; }
                    else if (role.Value == RoleEnum.Glitch) { playerRole += "<color=#" + Patches.Colors.Glitch.ToHtmlStringRGBA() + ">The Glitch</color> > "; }
                    else if (role.Value == RoleEnum.Jester) { playerRole += "<color=#" + Patches.Colors.Jester.ToHtmlStringRGBA() + ">Jester</color> > "; }
                    else if (role.Value == RoleEnum.Phantom) { playerRole += "<color=#" + Patches.Colors.Phantom.ToHtmlStringRGBA() + ">Phantom</color> > "; }
                    else if (role.Value == RoleEnum.Grenadier) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Grenadier</color> > "; }
                    else if (role.Value == RoleEnum.Janitor) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Janitor</color> > "; }
                    else if (role.Value == RoleEnum.Miner) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Miner</color> > "; }
                    else if (role.Value == RoleEnum.Morphling) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Morphling</color> > "; }
                    else if (role.Value == RoleEnum.Swooper) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Swooper</color> > "; }
                    else if (role.Value == RoleEnum.Camouflager) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Camouflager</color> > "; }
                    else if (role.Value == RoleEnum.Undertaker) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Undertaker</color> > "; }
                    else if (role.Value == RoleEnum.Haunter) { playerRole += "<color=#" + Patches.Colors.Haunter.ToHtmlStringRGBA() + ">Haunter</color> > "; }
                    else if (role.Value == RoleEnum.Veteran) { playerRole += "<color=#" + Patches.Colors.Veteran.ToHtmlStringRGBA() + ">Veteran</color> > "; }
                    else if (role.Value == RoleEnum.Amnesiac) { playerRole += "<color=#" + Patches.Colors.Amnesiac.ToHtmlStringRGBA() + ">Amnesiac</color> > "; }
                    else if (role.Value == RoleEnum.Shifter) { playerRole += "<color=#" + Patches.Colors.Shifter.ToHtmlStringRGBA() + ">Shifter</color> > "; }
                    else if (role.Value == RoleEnum.Juggernaut) { playerRole += "<color=#" + Patches.Colors.Juggernaut.ToHtmlStringRGBA() + ">Juggernaut</color> > "; }
                    else if (role.Value == RoleEnum.Tracker) { playerRole += "<color=#" + Patches.Colors.Tracker.ToHtmlStringRGBA() + ">Tracker</color> > "; }
                    else if (role.Value == RoleEnum.Transporter) { playerRole += "<color=#" + Patches.Colors.Transporter.ToHtmlStringRGBA() + ">Transporter</color> > "; }
                    else if (role.Value == RoleEnum.Traitor) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Traitor</color> > "; }
                    else if (role.Value == RoleEnum.Medium) { playerRole += "<color=#" + Patches.Colors.Medium.ToHtmlStringRGBA() + ">Medium</color> > "; }
                    else if (role.Value == RoleEnum.Trapper) { playerRole += "<color=#" + Patches.Colors.Trapper.ToHtmlStringRGBA() + ">Trapper</color> > "; }
                    else if (role.Value == RoleEnum.Survivor) { playerRole += "<color=#" + Patches.Colors.Survivor.ToHtmlStringRGBA() + ">Survivor</color> > "; }
                    else if (role.Value == RoleEnum.GuardianAngel) { playerRole += "<color=#" + Patches.Colors.GuardianAngel.ToHtmlStringRGBA() + ">Guardian Angel</color> > "; }
                    else if (role.Value == RoleEnum.Mystic) { playerRole += "<color=#" + Patches.Colors.Mystic.ToHtmlStringRGBA() + ">Mystic</color> > "; }
                    else if (role.Value == RoleEnum.Blackmailer) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Blackmailer</color> > "; }
                    else if (role.Value == RoleEnum.Plaguebearer) { playerRole += "<color=#" + Patches.Colors.Plaguebearer.ToHtmlStringRGBA() + ">Plaguebearer</color> > "; }
                    else if (role.Value == RoleEnum.Pestilence) { playerRole += "<color=#" + Patches.Colors.Pestilence.ToHtmlStringRGBA() + ">Pestilence</color> > "; }
                    else if (role.Value == RoleEnum.Werewolf) { playerRole += "<color=#" + Patches.Colors.Werewolf.ToHtmlStringRGBA() + ">Werewolf</color> > "; }
                    else if (role.Value == RoleEnum.Detective) { playerRole += "<color=#" + Patches.Colors.Detective.ToHtmlStringRGBA() + ">Detective</color> > "; }
                    else if (role.Value == RoleEnum.Escapist) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Escapist</color> > "; }
                    else if (role.Value == RoleEnum.Imitator) { playerRole += "<color=#" + Patches.Colors.Imitator.ToHtmlStringRGBA() + ">Imitator</color> > "; }
                    else if (role.Value == RoleEnum.Bomber) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Bomber</color> > "; }
                    else if (role.Value == RoleEnum.Doomsayer) { playerRole += "<color=#" + Patches.Colors.Doomsayer.ToHtmlStringRGBA() + ">Doomsayer</color> > "; }
                    else if (role.Value == RoleEnum.Vampire) { playerRole += "<color=#" + Patches.Colors.Vampire.ToHtmlStringRGBA() + ">Vampire</color> > "; }
                    else if (role.Value == RoleEnum.Prosecutor) { playerRole += "<color=#" + Patches.Colors.Prosecutor.ToHtmlStringRGBA() + ">Prosecutor</color> > "; }
                    else if (role.Value == RoleEnum.Warlock) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Warlock</color> > "; }
                    else if (role.Value == RoleEnum.Oracle) { playerRole += "<color=#" + Patches.Colors.Oracle.ToHtmlStringRGBA() + ">Oracle</color> > "; }
                    else if (role.Value == RoleEnum.Venerer) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Venerer</color> > "; }
                    else if (role.Value == RoleEnum.Aurial) { playerRole += "<color=#" + Patches.Colors.Aurial.ToHtmlStringRGBA() + ">Aurial</color> > "; }
                    else if (role.Value == RoleEnum.Politician) { playerRole += "<color=#" + Patches.Colors.Politician.ToHtmlStringRGBA() + ">Politician</color> > "; }
                    else if (role.Value == RoleEnum.Warden) { playerRole += "<color=#" + Patches.Colors.Warden.ToHtmlStringRGBA() + ">Warden</color> > "; }
                    else if (role.Value == RoleEnum.Hypnotist) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Hypnotist</color> > "; }
                    else if (role.Value == RoleEnum.Jailor) { playerRole += "<color=#" + Patches.Colors.Jailor.ToHtmlStringRGBA() + ">Jailor</color> > "; }
                    else if (role.Value == RoleEnum.SoulCollector) { playerRole += "<color=#" + Patches.Colors.SoulCollector.ToHtmlStringRGBA() + ">Soul Collector</color> > "; }
                    else if (role.Value == RoleEnum.Lookout) { playerRole += "<color=#" + Patches.Colors.Lookout.ToHtmlStringRGBA() + ">Lookout</color> > "; }
                    else if (role.Value == RoleEnum.Scavenger) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Scavenger</color> > "; }
                    else if (role.Value == RoleEnum.Deputy) { playerRole += "<color=#" + Patches.Colors.Deputy.ToHtmlStringRGBA() + ">Deputy</color> > "; }
                }
                playerRole = playerRole.Remove(playerRole.Length - 3);

                if (playerControl.Is(ModifierEnum.Giant)) playerRole += " (<color=#" + Patches.Colors.Giant.ToHtmlStringRGBA() + ">Giant</color>)";
                else if (playerControl.Is(ModifierEnum.ButtonBarry)) playerRole += " (<color=#" + Patches.Colors.ButtonBarry.ToHtmlStringRGBA() + ">Button Barry</color>)";
                else if (playerControl.Is(ModifierEnum.Aftermath)) playerRole += " (<color=#" + Patches.Colors.Aftermath.ToHtmlStringRGBA() + ">Aftermath</color>)";
                else if (playerControl.Is(ModifierEnum.Bait)) playerRole += " (<color=#" + Patches.Colors.Bait.ToHtmlStringRGBA() + ">Bait</color>)";
                else if (playerControl.Is(ModifierEnum.Diseased)) playerRole += " (<color=#" + Patches.Colors.Diseased.ToHtmlStringRGBA() + ">Diseased</color>)";
                else if (playerControl.Is(ModifierEnum.Flash)) playerRole += " (<color=#" + Patches.Colors.Flash.ToHtmlStringRGBA() + ">Flash</color>)";
                else if (playerControl.Is(ModifierEnum.Tiebreaker)) playerRole += " (<color=#" + Patches.Colors.Tiebreaker.ToHtmlStringRGBA() + ">Tiebreaker</color>)";
                else if (playerControl.Is(ModifierEnum.Torch)) playerRole += " (<color=#" + Patches.Colors.Torch.ToHtmlStringRGBA() + ">Torch</color>)";
                else if (playerControl.Is(ModifierEnum.Lover)) playerRole += " (<color=#" + Patches.Colors.Lovers.ToHtmlStringRGBA() + ">Lover</color>)";
                else if (playerControl.Is(ModifierEnum.Sleuth)) playerRole += " (<color=#" + Patches.Colors.Sleuth.ToHtmlStringRGBA() + ">Sleuth</color>)";
                else if (playerControl.Is(ModifierEnum.Radar)) playerRole += " (<color=#" + Patches.Colors.Radar.ToHtmlStringRGBA() + ">Radar</color>)";
                else if (playerControl.Is(ModifierEnum.Disperser)) playerRole += " (<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Disperser</color>)";
                else if (playerControl.Is(ModifierEnum.Multitasker)) playerRole += " (<color=#" + Patches.Colors.Multitasker.ToHtmlStringRGBA() + ">Multitasker</color>)";
                else if (playerControl.Is(ModifierEnum.DoubleShot)) playerRole += " (<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Double Shot</color>)";
                else if (playerControl.Is(ModifierEnum.Underdog)) playerRole += " (<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Underdog</color>)";
                else if (playerControl.Is(ModifierEnum.Frosty)) playerRole += " (<color=#" + Patches.Colors.Frosty.ToHtmlStringRGBA() + ">Frosty</color>)";
                else if (playerControl.Is(ModifierEnum.SixthSense)) playerRole += " (<color=#" + Patches.Colors.SixthSense.ToHtmlStringRGBA() + ">Sixth Sense</color>)";
                else if (playerControl.Is(ModifierEnum.Shy)) playerRole += " (<color=#" + Patches.Colors.Shy.ToHtmlStringRGBA() + ">Shy</color>)";
                else if (playerControl.Is(ModifierEnum.Mini)) playerRole += " (<color=#" + Patches.Colors.Mini.ToHtmlStringRGBA() + ">Mini</color>)";
                else if (playerControl.Is(ModifierEnum.Saboteur)) playerRole += " (<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Saboteur</color>)";

                var player = Role.GetRole(playerControl);
                if (playerControl.Is(RoleEnum.Phantom) || playerControl.Is(Faction.Crewmates))
                {
                    if ((player.TotalTasks - player.TasksLeft)/player.TotalTasks == 1) playerRole += " | Tasks: <color=#" + Color.green.ToHtmlStringRGBA() + $">{player.TotalTasks - player.TasksLeft}/{player.TotalTasks}</color>";
                    else playerRole += $" | Tasks: {player.TotalTasks - player.TasksLeft}/{player.TotalTasks}";
                }
                if (player.Kills > 0 && !playerControl.Is(Faction.Crewmates))
                {
                    playerRole += " |<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + $"> Kills: {player.Kills}</color>";
                }
                if (player.CorrectKills > 0)
                {
                    playerRole += " |<color=#" + Color.green.ToHtmlStringRGBA() + $"> Correct Kills: {player.CorrectKills}</color>";
                }
                if (player.IncorrectKills > 0)
                {
                    playerRole += " |<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + $"> Incorrect Kills: {player.IncorrectKills}</color>";
                }
                if (player.CorrectAssassinKills > 0)
                {
                    playerRole += " |<color=#" + Color.green.ToHtmlStringRGBA() + $"> Correct Guesses: {player.CorrectAssassinKills}</color>";
                }
                if (player.IncorrectAssassinKills > 0)
                {
                    playerRole += " |<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + $"> Incorrect Guesses: {player.IncorrectAssassinKills}</color>";
                }

                var playerName = "";
                foreach (var winner in EndGameResult.CachedWinners)
                {
                    if (winner.PlayerName == playerControl.Data.PlayerName) playerName += $"<color=#EFBF04>{playerControl.Data.PlayerName}</color>";
                }
                if (!CustomGameOptions.NeutralEvilWinEndsGame)
                {
                    if (playerControl.Is(RoleEnum.Doomsayer))
                    {
                        var doom = Role.GetRole<Doomsayer>(playerControl);
                        if (doom.WonByGuessing)
                        {
                            AdditionalTempData.otherWinners.Add(new AdditionalTempData.Winners() { PlayerName = doom.Player.Data.PlayerName, Role = RoleEnum.Doomsayer });
                            playerName += $"<color=#EFBF04>{playerControl.Data.PlayerName}</color>";
                        }
                    }
                    if (playerControl.Is(RoleEnum.Executioner))
                    {
                        var exe = Role.GetRole<Executioner>(playerControl);
                        if (exe.TargetVotedOut)
                        {
                            AdditionalTempData.otherWinners.Add(new AdditionalTempData.Winners() { PlayerName = exe.Player.Data.PlayerName, Role = RoleEnum.Executioner });
                            playerName += $"<color=#EFBF04>{playerControl.Data.PlayerName}</color>";
                        }
                    }
                    if (playerControl.Is(RoleEnum.Jester))
                    {
                        var jest = Role.GetRole<Jester>(playerControl);
                        if (jest.VotedOut)
                        {
                            AdditionalTempData.otherWinners.Add(new AdditionalTempData.Winners() { PlayerName = jest.Player.Data.PlayerName, Role = RoleEnum.Jester });
                            playerName += $"<color=#EFBF04>{playerControl.Data.PlayerName}</color>";
                        }
                    }
                    if (playerControl.Is(RoleEnum.Phantom))
                    {
                        var phan = Role.GetRole<Phantom>(playerControl);
                        if (phan.CompletedTasks)
                        {
                            AdditionalTempData.otherWinners.Add(new AdditionalTempData.Winners() { PlayerName = phan.Player.Data.PlayerName, Role = RoleEnum.Phantom });
                            playerName += $"<color=#EFBF04>{playerControl.Data.PlayerName}</color>";
                        }
                    }
                    if (playerControl.Is(RoleEnum.SoulCollector))
                    {
                        var sc = Role.GetRole<SoulCollector>(playerControl);
                        if (sc.CollectedSouls)
                        {
                            AdditionalTempData.otherWinners.Add(new AdditionalTempData.Winners() { PlayerName = sc.Player.Data.PlayerName, Role = RoleEnum.SoulCollector });
                            playerName += $"<color=#EFBF04>{playerControl.Data.PlayerName}</color>";
                        }
                    }
                }
                if (playerName == "") playerName += playerControl.Data.PlayerName;

                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerName, Role = playerRole });
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch {

        public static void Postfix(EndGameManager __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f); 
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();
            roleSummaryText.AppendLine("End game summary:");
            foreach(var data in AdditionalTempData.playerRoles) {
                var role = string.Join(" ", data.Role);
                roleSummaryText.AppendLine($"{data.PlayerName} - {role}");
            }

            if (AdditionalTempData.otherWinners.Count != 0)
            {
                roleSummaryText.AppendLine("\n\n\nOther Winners:");
                foreach (var data in AdditionalTempData.otherWinners)
                {
                    if (data.Role == RoleEnum.Doomsayer) roleSummaryText.AppendLine("<color=#" + Patches.Colors.Doomsayer.ToHtmlStringRGBA() + $">{data.PlayerName}</color>");
                    else if (data.Role == RoleEnum.Executioner) roleSummaryText.AppendLine("<color=#" + Patches.Colors.Executioner.ToHtmlStringRGBA() + $">{data.PlayerName}</color>");
                    else if (data.Role == RoleEnum.Jester) roleSummaryText.AppendLine("<color=#" + Patches.Colors.Jester.ToHtmlStringRGBA() + $">{data.PlayerName}</color>");
                    else if (data.Role == RoleEnum.Phantom) roleSummaryText.AppendLine("<color=#" + Patches.Colors.Phantom.ToHtmlStringRGBA() + $">{data.PlayerName}</color>");
                    else if (data.Role == RoleEnum.SoulCollector) roleSummaryText.AppendLine("<color=#" + Patches.Colors.SoulCollector.ToHtmlStringRGBA() + $">{data.PlayerName}</color>");
                }
            }

            TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
            roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.fontSizeMin = 1.5f;
            roleSummaryTextMesh.fontSizeMax = 1.5f;
            roleSummaryTextMesh.fontSize = 1.5f;
             
            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText.ToString();

            AdditionalTempData.clear();
        }
    }
}