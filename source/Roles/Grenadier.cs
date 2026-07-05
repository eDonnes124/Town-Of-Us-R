using System;
using UnityEngine;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.GrenadierMod;

namespace TownOfUs.Roles
{
    public class Grenadier : Role
    {
        public KillButton _flashButton;
        public bool Enabled;
        public DateTime LastFlashed;
        public float TimeRemaining;
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> closestPlayers = null;

        static readonly Color normalVision = new Color(0.6f, 0.6f, 0.6f, 0f);
        public Il2CppSystem.Collections.Generic.List<PlayerControl> flashedPlayers = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        public float flashPercent = 0f;

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            ImpostorText = () => "Flashbang The Crewmates";
            TaskText = () => "Flash the Crewmates to get sneaky kills";
            Color = Patches.Colors.Impostor;
            LastFlashed = DateTime.UtcNow;
            RoleType = RoleEnum.Grenadier;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public bool Flashed => TimeRemaining > 0f;


        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlashed;
            var num = CustomGameOptions.GrenadeCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void StartFlash()
        {
            closestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius, true);
            flashedPlayers = closestPlayers;
            Flash();
        }

        public void Flash()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var sabActive = system.AnyActive;

            if (flashedPlayers.Contains(PlayerControl.LocalPlayer))
            {
                if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && (!sabActive))
                {
                    float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;
                    if (FlashUnFlash.ShouldPlayerBeBlinded(PlayerControl.LocalPlayer) || FlashUnFlash.ShouldPlayerBeDimmed(PlayerControl.LocalPlayer))
                    {
                        flashPercent = fade;
                        try
                        {
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        flashPercent = 0f;
                    }
                }
                else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && (!sabActive))
                {
                    if (FlashUnFlash.ShouldPlayerBeBlinded(PlayerControl.LocalPlayer) || FlashUnFlash.ShouldPlayerBeDimmed(PlayerControl.LocalPlayer))
                    {
                        flashPercent = 1f;
                        try
                        {
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        flashPercent = 0f;
                    }
                }
                else if (TimeRemaining < 0.5f && (!sabActive))
                {
                    float fade2 = TimeRemaining * 2.0f;
                    if (FlashUnFlash.ShouldPlayerBeBlinded(PlayerControl.LocalPlayer) || FlashUnFlash.ShouldPlayerBeDimmed(PlayerControl.LocalPlayer))
                    {
                        flashPercent = fade2;
                    }
                    else
                    {
                        flashPercent = 0f;
                    }
                }
                else
                {
                    ((Renderer)HudManager.Instance.FullScreen).enabled = true;
                    ((Renderer)HudManager.Instance.FullScreen).gameObject.active = true;
                    HudManager.Instance.FullScreen.color = normalVision;
                    flashPercent = 0f;
                    TimeRemaining = 0.0f;
                }
            }

            if (TimeRemaining > 0.5f)
            {
                try
                {
                    if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                    {
                        MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                    }
                }
                catch { }
            }
        }

        public void UnFlash()
        {
            Enabled = false;
            flashPercent = 0f;
            LastFlashed = DateTime.UtcNow;
            ((Renderer)HudManager.Instance.FullScreen).enabled = true;
            HudManager.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }
    }
}