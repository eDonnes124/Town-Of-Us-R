using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using Reactor.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Patches {

    static class AdditionalTempData {
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();

        public static void clear() {
            playerRoles.Clear();
        }

        internal class PlayerRoleInfo {
            public string PlayerName { get; set; }
            public string Role {get;set;}
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch {

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            AdditionalTempData.clear();
            var playerRole = "";
            // Theres a better way of doing this e.g. switch statement or dictionary. But this works for now.
            foreach (var playerControl in PlayerControl.AllPlayerControls)
            {
                playerRole = "";
                foreach (var role in Role.RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    if (role.Value == RoleEnum.Crewmate) {playerRole += "<color=#"+Patches.Colors.Crewmate.ToHtmlStringRGBA()+">Crewmate</color> > ";}
                    else if (role.Value == RoleEnum.Impostor) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Impostor</color> > ";}
                    else if (role.Value == RoleEnum.Altruist) {playerRole += "<color=#"+Patches.Colors.Altruist.ToHtmlStringRGBA()+">Altruist</color> > ";}
                    else if (role.Value == RoleEnum.Engineer) {playerRole += "<color=#"+Patches.Colors.Engineer.ToHtmlStringRGBA()+">Engineer</color> > ";}
                    else if (role.Value == RoleEnum.Investigator) {playerRole += "<color=#"+Patches.Colors.Investigator.ToHtmlStringRGBA()+">Investigator</color> > ";}
                    else if (role.Value == RoleEnum.Mayor) {playerRole += "<color=#"+Patches.Colors.Mayor.ToHtmlStringRGBA()+">Mayor</color> > ";}
                    else if (role.Value == RoleEnum.Medic) {playerRole += "<color=#"+Patches.Colors.Medic.ToHtmlStringRGBA()+">Medic</color> > ";}
                    else if (role.Value == RoleEnum.Sheriff) {playerRole += "<color=#"+Patches.Colors.Sheriff.ToHtmlStringRGBA()+">Sheriff</color> > ";}
                    else if (role.Value == RoleEnum.Swapper) {playerRole += "<color=#"+Patches.Colors.Swapper.ToHtmlStringRGBA()+">Swapper</color> > ";}
                    else if (role.Value == RoleEnum.TimeLord) {playerRole += "<color=#"+Patches.Colors.TimeLord.ToHtmlStringRGBA()+">Time Lord</color> > ";}
                    else if (role.Value == RoleEnum.Seer) {playerRole += "<color=#"+Patches.Colors.Seer.ToHtmlStringRGBA()+">Seer</color> > ";}
                    else if (role.Value == RoleEnum.Snitch) {playerRole += "<color=#"+Patches.Colors.Snitch.ToHtmlStringRGBA()+">Snitch</color> > ";}
                    else if (role.Value == RoleEnum.Spy) {playerRole += "<color=#"+Patches.Colors.Spy.ToHtmlStringRGBA()+">Spy</color> > ";}
                    else if (role.Value == RoleEnum.Vigilante) {playerRole += "<color=#"+Patches.Colors.Vigilante.ToHtmlStringRGBA()+">Vigilante</color> > "; }
                    else if (role.Value == RoleEnum.Arsonist) {playerRole += "<color=#"+Patches.Colors.Arsonist.ToHtmlStringRGBA()+">Arsonist</color> > ";}
                    else if (role.Value == RoleEnum.Executioner) {playerRole += "<color=#"+Patches.Colors.Executioner.ToHtmlStringRGBA()+">Executioner</color> > ";}
                    else if (role.Value == RoleEnum.Glitch) {playerRole += "<color=#"+Patches.Colors.Glitch.ToHtmlStringRGBA()+">The Glitch</color> > ";}
                    else if (role.Value == RoleEnum.Jester) {playerRole += "<color=#"+Patches.Colors.Jester.ToHtmlStringRGBA()+">Jester</color> > ";}
                    else if (role.Value == RoleEnum.Phantom) {playerRole += "<color=#"+Patches.Colors.Phantom.ToHtmlStringRGBA()+">Phantom</color> > ";}
                    //else if (role.Value == RoleEnum.Assassin) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Assassin</color> > ";}
                    else if (role.Value == RoleEnum.Camouflager) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Camouflager</color> > ";}
                    else if (role.Value == RoleEnum.Grenadier) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Grenadier</color> > ";}
                    else if (role.Value == RoleEnum.Janitor) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Janitor</color> > ";}
                    else if (role.Value == RoleEnum.Miner) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Miner</color> > ";}
                    else if (role.Value == RoleEnum.Morphling) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Morphling</color> > ";}
                    else if (role.Value == RoleEnum.Swooper) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Swooper</color> > ";}
                    else if (role.Value == RoleEnum.Underdog) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Underdog</color> > ";}
                    else if (role.Value == RoleEnum.Undertaker) {playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Undertaker</color> > "; }
                    else if (role.Value == RoleEnum.Haunter) { playerRole += "<color=#"+Patches.Colors.Haunter.ToHtmlStringRGBA()+">Haunter</color> > "; }
                    else if (role.Value == RoleEnum.Grenadier) { playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Grenadier</color> > "; }
                    else if (role.Value == RoleEnum.Veteran) { playerRole += "<color=#"+Patches.Colors.Veteran.ToHtmlStringRGBA()+">Veteran</color> > "; }
                    else if (role.Value == RoleEnum.Amnesiac) { playerRole += "<color=#"+Patches.Colors.Amnesiac.ToHtmlStringRGBA()+">Amnesiac</color> > "; }
                    else if (role.Value == RoleEnum.Juggernaut) { playerRole += "<color=#"+Patches.Colors.Juggernaut.ToHtmlStringRGBA()+">Juggernaut</color> > "; }
                    else if (role.Value == RoleEnum.Tracker) { playerRole += "<color=#"+Patches.Colors.Tracker.ToHtmlStringRGBA()+">Tracker</color> > "; }
                    else if (role.Value == RoleEnum.Poisoner) { playerRole += "<color=#"+Patches.Colors.Impostor.ToHtmlStringRGBA()+">Poisoner</color> > "; }
                    else if (role.Value == RoleEnum.Transporter) { playerRole += "<color=#" + Patches.Colors.Transporter.ToHtmlStringRGBA() + ">Transporter</color> > "; }
                    else if (role.Value == RoleEnum.Traitor) { playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Traitor</color> > "; }
                }
                playerRole = playerRole.Remove(playerRole.Length - 3);

                if (playerControl.Is(ModifierEnum.Giant)) {
                    playerRole += " (<color=#" + Patches.Colors.Giant.ToHtmlStringRGBA() + ">Giant</color>)";
                } else if (playerControl.Is(ModifierEnum.ButtonBarry)) {
                    playerRole += " (<color=#" + Patches.Colors.ButtonBarry.ToHtmlStringRGBA() + ">Button Barry</color>)";
                } else if (playerControl.Is(ModifierEnum.Bait)) {
                    playerRole += " (<color=#" + Patches.Colors.Bait.ToHtmlStringRGBA() + ">Bait</color>)";
                } else if (playerControl.Is(ModifierEnum.Diseased)) {
                    playerRole += " (<color=#" + Patches.Colors.Diseased.ToHtmlStringRGBA() + ">Diseased</color>)";
                } else if (playerControl.Is(ModifierEnum.Drunk)) {
                    playerRole += " (<color=#" + Patches.Colors.Drunk.ToHtmlStringRGBA() + ">Drunk</color>)";
                } else if (playerControl.Is(ModifierEnum.Flash)) {
                    playerRole += " (<color=#" + Patches.Colors.Flash.ToHtmlStringRGBA() + ">Flash</color>)";
                } else if (playerControl.Is(ModifierEnum.Tiebreaker)) {
                    playerRole += " (<color=#" + Patches.Colors.Tiebreaker.ToHtmlStringRGBA() + ">Tiebreaker</color>)";
                } else if (playerControl.Is(ModifierEnum.Torch)) {
                    playerRole += " (<color=#" + Patches.Colors.Torch.ToHtmlStringRGBA() + ">Torch</color>)";
                } else if (playerControl.Is(ModifierEnum.Lover)) {
                    playerRole += " (<color=#" + Patches.Colors.Lovers.ToHtmlStringRGBA() + ">Lover</color>)";
                } else if (playerControl.Is(ModifierEnum.Sleuth)) {
                    playerRole += " (<color=#" + Patches.Colors.Sleuth.ToHtmlStringRGBA() + ">Sleuth</color>)";
                }  
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Role = playerRole });
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch {
        public static void Postfix(EndGameManager __instance) {
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