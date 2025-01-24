using HarmonyLib;
using System;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Modifiers.UnderdogMod;
using TownOfUs.Patches;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.ImpostorRoles.ScavengerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Scavenger)) return;
            var role = Role.GetRole<Scavenger>(PlayerControl.LocalPlayer);

            if (role.ScavengeCooldown == null)
            {
                role.ScavengeCooldown = UnityEngine.Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.ScavengeCooldown.gameObject.SetActive(false);
                role.ScavengeCooldown.transform.localPosition = new Vector3(
                    role.ScavengeCooldown.transform.localPosition.x + 0.26f,
                    role.ScavengeCooldown.transform.localPosition.y + 0.29f,
                    role.ScavengeCooldown.transform.localPosition.z);
                role.ScavengeCooldown.transform.localScale *= 0.65f;
                role.ScavengeCooldown.alignment = TMPro.TextAlignmentOptions.Right;
                role.ScavengeCooldown.fontStyle = TMPro.FontStyles.Bold;
                role.ScavengeCooldown.enableWordWrapping = false;
            }
            if (role.ScavengeCooldown != null)
            {
                role.ScavengeCooldown.text = Convert.ToInt32(Math.Round(role.ScavengeTimer())).ToString();
            }
            role.ScavengeCooldown.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && role.Scavenging);

            if (role.Scavenging && PlayerControl.LocalPlayer.moveable && __instance.KillButton.currentTarget != null)
            {
                role.ScavengeCooldown.color = Palette.EnabledColor;
                role.ScavengeCooldown.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.ScavengeCooldown.color = Palette.DisabledClear;
                role.ScavengeCooldown.material.SetFloat("_Desat", 1f);
            }

            if ((role.ScavengeTimer() == 0f || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead) && role.Scavenging)
            {
                role.StopScavenge();

                if (role.Player.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    role.Player.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else role.Player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }

            if (!role.GameStarted && PlayerControl.LocalPlayer.killTimer > 0f) role.GameStarted = true;

            if (PlayerControl.LocalPlayer.killTimer == 0f && !role.Scavenging && role.GameStarted && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.Scavenging = true;
                role.ScavengeEnd = DateTime.UtcNow.AddSeconds(CustomGameOptions.ScavengeDuration);
                role.Target = role.GetClosestPlayer();
                role.RegenTask();
            }

            if (role.Target != null)
            {
                if (role.PreyArrow == null)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    renderer.color = Colors.Impostor;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrow.target = role.Target.transform.position;
                    role.PreyArrow = arrow;
                }
                role.PreyArrow.target = role.Target.transform.position;
            }

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                if (role.Target != null && !role.Target.Data.IsDead && !role.Target.Data.Disconnected)
                {
                    if (role.Target.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        role.Target.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                    {
                        var colour = new Color(0.45f, 0f, 0f);
                        if (role.Target.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(role.Target).Opacity;
                        role.Target.nameText().color = colour;
                    }
                    else role.Target.nameText().color = Color.clear;
                }
            }
        }
    }
}