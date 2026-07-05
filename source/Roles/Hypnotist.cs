using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Hypnotist : Role
    {
        public KillButton _hypnotiseButton;
        
        public PlayerControl ClosestPlayer;
        public List<byte> HypnotisedPlayers = new List<byte>();
        public DateTime LastHypnotised { get; set; }
        public bool HysteriaActive;

        public Hypnotist(PlayerControl player) : base(player)
        {
            Name = "Hypnotist";
            ImpostorText = () => "Hypnotize Crewmates";
            TaskText = () => "Hypnotize Crewmates and drive them insane";
            Color = Patches.Colors.Impostor;
            LastHypnotised = DateTime.UtcNow;
            RoleType = RoleEnum.Hypnotist;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            HysteriaActive = false;
        }
        public GameObject HysteriaButton = new GameObject();

        public KillButton HypnotiseButton
        {
            get => _hypnotiseButton;
            set
            {
                _hypnotiseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float HypnotiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastHypnotised;
            var num = CustomGameOptions.HypnotiseCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Hysteria()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead || !HypnotisedPlayers.Contains(PlayerControl.LocalPlayer.PlayerId)) return;

            PlayerControl.LocalPlayer.SetOutfit(CustomPlayerOutfitType.Default);
            UnHysteria();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead || player.Data.Disconnected || player == PlayerControl.LocalPlayer) continue;
                int hidden = Random.RandomRangeInt(0, 3);
                if (hidden == 0)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Morph, PlayerControl.LocalPlayer.Data.DefaultOutfit);
                }
                else if (hidden == 1)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new NetworkedPlayerInfo.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " ",
                        PetId = ""
                    });
                    PlayerMaterial.SetColors(UnityEngine.Color.grey, player.myRend());
                    player.nameText().color = UnityEngine.Color.clear;
                    player.cosmetics.colorBlindText.color = UnityEngine.Color.clear;
                }
                else
                {
                    player.SetOutfit(CustomPlayerOutfitType.Swooper, new NetworkedPlayerInfo.PlayerOutfit()
                    {
                        ColorId = player.CurrentOutfit.ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " ",
                        PetId = ""
                    });
                    player.myRend().color = UnityEngine.Color.clear;
                    player.nameText().color = UnityEngine.Color.clear;
                    player.cosmetics.colorBlindText.color = UnityEngine.Color.clear;
                }
            }
        }

        public void UnHysteria()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Swooper) player.myRend().color = UnityEngine.Color.white;
                if (player.Is(RoleEnum.Swooper))
                {
                    var swooper = GetRole<Swooper>(player);
                    if (swooper.IsSwooped) continue;
                }
                else if (player.Is(RoleEnum.Venerer))
                {
                    var venerer = GetRole<Venerer>(player);
                    if (venerer.IsCamouflaged) continue;
                }
                else if (player.Is(RoleEnum.Morphling))
                {
                    var morphling = GetRole<Morphling>(player);
                    if (morphling.Morphed) continue;
                }
                else if (player.Is(RoleEnum.Glitch))
                {
                    var glitch = GetRole<Glitch>(player);
                    if (glitch.IsUsingMimic) continue;
                }
                else if (CamouflageUnCamouflage.IsCamoed) continue;
                player.SetOutfit(CustomPlayerOutfitType.Default);
            }
        }
    }
}