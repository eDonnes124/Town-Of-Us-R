using System;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Venerer : Role
    {
        public KillButton _abilityButton;
        public bool Enabled;
        public DateTime LastCamouflaged;
        public float TimeRemaining;
        public float KillsAtStartAbility;

        public Venerer(PlayerControl player) : base(player)
        {
            Name = "Venerer";
            ImpostorText = () => "With Each Kill Your Ability Becomes Stronger";
            TaskText = () => "Kill players to unlock ability perks";
            Color = Patches.Colors.Impostor;
            LastCamouflaged = DateTime.UtcNow;
            RoleType = RoleEnum.Venerer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public bool IsCamouflaged => TimeRemaining > 0f;

        public KillButton AbilityButton
        {
            get => _abilityButton;
            set
            {
                _abilityButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float AbilityTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.AbilityCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ability()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision) return;
            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
                PlayerMaterial.SetColors(Color.grey, Player.myRend());
                Player.nameText().color = Color.clear;
                Player.cosmetics.colorBlindText.color = Color.clear;
            }
        }


        public void StopAbility()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            if (!CamouflageUnCamouflage.IsCamoed) Utils.Unmorph(Player);
        }
    }
}