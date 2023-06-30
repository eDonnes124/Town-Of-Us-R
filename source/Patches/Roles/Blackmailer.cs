using System;
using System.Linq;
using AmongUs.GameOptions;
using Hazel;
using TownOfUs.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Blackmailer : Role
    {
        public KillButton _blackmailButton;
        
        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public DateTime LastBlackmailed { get; set; }

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            ImpostorText = () => "Silence Crewmates During Meetings";
            TaskText = () => "Silence a crewmate for the next meeting";
            Color = Patches.Colors.Impostor;
            LastBlackmailed = DateTime.UtcNow;
            RoleType = RoleEnum.Blackmailer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = CustomGameOptions.BlackmailCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void HudManagerUpdate(HudManager __instance)
        {
            if (BlackmailButton == null)
            {
                BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                BlackmailButton.graphic.enabled = true;
                BlackmailButton.gameObject.SetActive(false);
            }

            BlackmailButton.graphic.sprite = TownOfUs.BlackmailSprite;
            BlackmailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(
                player => Blackmailed?.PlayerId != player.PlayerId
            ).ToList();

            Utils.SetTarget(ref ClosestPlayer, BlackmailButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notBlackmailed);

            BlackmailButton.SetCoolDown(BlackmailTimer(), CustomGameOptions.BlackmailCd);

            if (Blackmailed?.Data.IsDead == false && !Blackmailed.Data.Disconnected)
            {
                Blackmailed.myRend().material.SetFloat("_Outline", 1f);
                Blackmailed.myRend().material.SetColor("_OutlineColor", new Color(0.3f, 0.0f, 0.0f));
                if (Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                {
                    Blackmailed.nameText().color = new Color(0.3f, 0.0f, 0.0f);
                }
                else
                {
                    Blackmailed.nameText().color = Color.clear;
                }
            }

            var imps = PlayerControl.AllPlayerControls.ToArray().Where(
                player => player.Data.IsImpostor() && player != Blackmailed
            ).ToList();

            foreach (var imp in imps)
            {
                if (imp.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage ||
                    imp.GetCustomOutfitType() == CustomPlayerOutfitType.Swooper)
                {
                    imp.nameText().color = Color.clear;
                }
                else if (imp.nameText().color == Color.clear ||
                                    imp.nameText().color == new Color(0.3f, 0.0f, 0.0f))
                {
                    imp.nameText().color = Patches.Colors.Impostor;
                }
            }
        }

        protected override bool UseKillButton(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var target = ClosestPlayer;
            if (__instance == BlackmailButton)
            {
                if (!__instance.isActiveAndEnabled || ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (BlackmailTimer() != 0) return false;

                var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                if (interact[4])
                {
                    Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                    if (Blackmailed?.Data.IsImpostor() == true)
                    {
                        if (Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                            Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        {
                            Blackmailed.nameText().color = Patches.Colors.Impostor;
                        }
                        else
                        {
                            Blackmailed.nameText().color = Color.clear;
                        }
                    }
                    Blackmailed = target;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.Blackmail, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                BlackmailButton.SetCoolDown(0.01f, 1f);
                return false;
            }
            return true;
        }
    }
}