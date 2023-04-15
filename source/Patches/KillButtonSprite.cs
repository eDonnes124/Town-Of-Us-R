using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.Start))]
    public static class KillButtonAwake
    {
        public static void Prefix(KillButton __instance)
        {
            __instance.transform.Find("Text_TMP").gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillButtonSprite
    {
        private static Sprite Kill;

        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            bool flag;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer) || PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.SeerSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.MedicSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.DouseSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Altruist))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.ReviveSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.AlertSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.RememberSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.TrackSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.TransportSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.MediateSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.VestSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.ProtectSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.InfectSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) && CustomGameOptions.GameMode != GameMode.Cultist)
            {
                __instance.KillButton.graphic.sprite = TownOfUs.EngineerFix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.TrapSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.ExamineSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                __instance.KillButton.graphic.sprite = TownOfUs.SwoopSprite;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Kill";
                flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut);
            }
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors) &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
            {
                __instance.KillButton.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if (keyInt | controller && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class AbilityButtonUpdatePatch
        {
            public static bool Ran;
            static void Postfix(HudManager __instance)
            {
                if (__instance.AbilityButton == null) return;

                if (!GameManager.Instance.GameHasStarted)
                {
                    __instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    __instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }

                bool flag = false;
                var player = PlayerControl.LocalPlayer;
                var dead = player.Data.IsDead;

                if (player.Is(RoleEnum.Arsonist))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.IgniteSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Blackmailer))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.BlackmailSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Bomber))
                {
                    flag = true;
                }
                else if (player.Is(RoleEnum.Escapist))
                {
                    flag = true;
                }
                else if (player.Is(RoleEnum.Grenadier))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.FlashSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Janitor))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.JanitorClean;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Miner))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.MineSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Morphling))
                {
                    flag = true;
                }
                else if (player.Is(RoleEnum.Swooper))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.SwoopSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Undertaker))
                {
                    flag = true;
                }
                else if (player.Is(RoleEnum.Werewolf))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.RampageSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Whisperer))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.WhisperSprite;
                    flag = true;
                }
                else if (player.Is(RoleEnum.Necromancer))
                {
                    __instance.AbilityButton.graphic.sprite = TownOfUs.NecroReviveSprite;
                    flag = true;
                }

                var ghostRole = false;
                if (player.Is(RoleEnum.Haunter))
                {
                    var haunter = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (!haunter.Caught) ghostRole = true;
                }
                else if (player.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (!phantom.Caught) ghostRole = true;
                }

                __instance.AbilityButton.buttonLabelText.text = string.Empty;
                __instance.AbilityButton.usesRemainingText.text = string.Empty;
                __instance.AbilityButton.usesRemainingSprite.gameObject.SetActive(false);
                __instance.AbilityButton.gameObject
                              .SetActive((HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled)
                                        && !MeetingHud.Instance
                                        && !dead);

                if (flag && !dead)
                {
                    if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ActionQuaternary")) __instance.AbilityButton.DoClick();
                    return;
                }
                else
                {
                    if (dead
                        && (player.Data.RoleType != RoleTypes.ImpostorGhost || player.Data.RoleType != RoleTypes.CrewmateGhost)
                        && !ghostRole
                        && flag
                        && !Ran)
                    {
                        bool imp = player.Data.IsImpostor();
                        player.SetRole(imp ? RoleTypes.ImpostorGhost : RoleTypes.CrewmateGhost);
                        Ran = true;
                    }
                    player.Data.Role.InitializeAbilityButton();
                }
                __instance.AbilityButton.gameObject.SetActive(!ghostRole && Utils.ShowDeadBodies && !MeetingHud.Instance);

            }
        }
    }
}
