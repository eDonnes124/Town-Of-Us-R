using System;
using System.Collections.Generic;
using TownOfUs.Patches.ScreenEffects;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Aurial : Role
    {
        public Dictionary<byte,int> knownPlayerRoles = new Dictionary<byte,int>();
        public static Material ViewMat = TownOfUs.bundledAssets.Get<Material>("SoundV");
        public DateTime LastRadiated { get; set; }
        public DateTime CannotSeeDelay { get; set; }
        public bool NormalVision { get; set; } = false;
        public bool Loaded { get; set; } = false;
        public Aurial(PlayerControl player) : base(player)
        {
            Name = "Aurial";
            ImpostorText = () => "Radiate To See The Auras Of Other Players";
            TaskText = () => "Radiate to discover evildoers";
            Color = Patches.Colors.Aurial;
            RoleType = RoleEnum.Aurial;
            AddToRoleHistory(RoleType);

            LastRadiated = DateTime.UtcNow;
            if (Player == PlayerControl.LocalPlayer) ApplyEffect();
        }

        public float RadiateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRadiated;
            var num = CustomGameOptions.RadiateCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0f;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float SeeDelay()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - CannotSeeDelay;
            var num = CustomGameOptions.RadiateInvis * 1000f;
            if (num > CustomGameOptions.RadiateCooldown * 1000f) num = CustomGameOptions.RadiateCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0f;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void ApplyEffect()
        {
            CameraEffect.Initialize();
            CameraEffect.singleton.materials.Clear();
            CameraEffect.singleton.materials.Add(ViewMat);     
        }

        public void ClearEffect()
        {
            CameraEffect.singleton.materials.Clear();
        }
    }
}
