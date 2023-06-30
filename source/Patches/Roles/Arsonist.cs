using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Hazel;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public bool ArsonistWins;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public bool LastKiller = false;

        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);


        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            ImpostorText = () => "Douse Players And Ignite The Light";
            TaskText = () => "Douse players and ignite to kill all douses\nFake Tasks:";
            Color = Patches.Colors.Arsonist;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Arsonist;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(RoleEnum.Glitch) || x.Is(RoleEnum.Juggernaut) ||
                    x.Is(RoleEnum.Werewolf) || x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence))))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.ArsonistWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
            }

            return false;
        }


        public void Wins()
        {
            ArsonistWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var arsonistTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            arsonistTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = arsonistTeam;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            foreach (var playerId in DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected())
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            DousedPlayers.Clear();
        }

        protected override void HudManagerUpdate(HudManager __instance)
        {
            foreach (var playerId in DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;
                if (data?.Disconnected != false || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                    continue;

                player.myRend().material.SetColor("_VisorColor", Color);
                player.nameText().color = Color.black;
            }

            if (IgniteButton == null)
            {
                IgniteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                IgniteButton.graphic.enabled = true;
                IgniteButton.gameObject.SetActive(false);
            }

            IgniteButton.graphic.sprite = TownOfUs.IgniteSprite;
            IgniteButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            IgniteButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (!LastKiller || !CustomGameOptions.IgniteCdRemoved) IgniteButton.SetCoolDown(DouseTimer(), CustomGameOptions.DouseCd);
            else IgniteButton.SetCoolDown(0f, CustomGameOptions.DouseCd);
            if (DousedAlive < CustomGameOptions.MaxDoused)
            {
                __instance.KillButton.SetCoolDown(DouseTimer(), CustomGameOptions.DouseCd);
            }

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !DousedPlayers.Contains(player.PlayerId)
            ).ToList();
            var doused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => DousedPlayers.Contains(player.PlayerId)
            ).ToList();

            if (DousedAlive < CustomGameOptions.MaxDoused)
            {
                Utils.SetTarget(ref ClosestPlayerDouse, __instance.KillButton, float.NaN, notDoused);
            }

            if (DousedAlive > 0)
            {
                Utils.SetTarget(ref ClosestPlayerIgnite, IgniteButton, float.NaN, doused);
            }
        }

        protected override bool UseKillButton(KillButton __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;

            if (__instance == IgniteButton && DousedAlive > 0)
            {
                if (DouseTimer() == 0 || (LastKiller && CustomGameOptions.IgniteCdRemoved))
                {
                    if (ClosestPlayerIgnite == null) return false;
                    var distBetweenPlayers2 = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, ClosestPlayerIgnite);
                    var flag3 = distBetweenPlayers2 <
                                GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (!flag3) return false;
                    if (!DousedPlayers.Contains(ClosestPlayerIgnite.PlayerId)) return false;

                    var interact2 = Utils.Interact(PlayerControl.LocalPlayer, ClosestPlayerIgnite);
                    if (interact2[4]) Ignite();
                    if (interact2[0])
                    {
                        LastDoused = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact2[1])
                    {
                        LastDoused = DateTime.UtcNow;
                        LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DouseCd);
                        return false;
                    }
                    else if (interact2[3])
                    {
                        return false;
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }

            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (DousedAlive == CustomGameOptions.MaxDoused) return false;
            if (ClosestPlayerDouse == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, ClosestPlayerDouse);
            var flag2 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag2) return false;
            if (DousedPlayers.Contains(ClosestPlayerDouse.PlayerId)) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, ClosestPlayerDouse);
            if (interact[4]) DousedPlayers.Add(ClosestPlayerDouse.PlayerId);
            if (interact[0])
            {
                LastDoused = DateTime.UtcNow;
                return false;
            }
            else if (interact[1])
            {
                LastDoused = DateTime.UtcNow;
                LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DouseCd);
                return false;
            }
            else if (interact[3])
            {
                return false;
            }

            return false;
        }
    }
}
