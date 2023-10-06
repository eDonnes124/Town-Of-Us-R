using UnityEngine;
using Reactor.Utilities.Extensions;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using System.Reflection;
using Hazel;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Bomber : Role

    {
        public KillButton _plantButton;
        public float TimeRemaining;
        public bool Enabled = false;
        public bool Detonated = true;
        public Vector3 DetonatePoint;
        public Bomb Bomb = new Bomb();
        public static Material bombMaterial = TownOfUs.bundledAssets.Get<Material>("bomb");
        public DateTime StartingCooldown { get; set; }

        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            ImpostorText = () => "Plant Bombs To Kill Multiple Crewmates At Once";
            TaskText = () => "Plant bombs to kill crewmates";
            Color = Palette.ImpostorRed;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Bomber;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton PlantButton
        {
            get => _plantButton;
            set
            {
                _plantButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public bool Detonating => TimeRemaining > 0f;
        public void DetonateTimer()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (MeetingHud.Instance) Detonated = true;
            if (TimeRemaining <= 0 && !Detonated)
            {
                var bomber = GetRole<Bomber>(PlayerControl.LocalPlayer);
                bomber.Bomb.ClearBomb();
                DetonateKillStart();
            }
        }
        public void DetonateKillStart()
        {
            Detonated = true;
            var playersToDie = Utils.GetClosestPlayers(DetonatePoint, CustomGameOptions.DetonateRadius, false);
            playersToDie = Shuffle(playersToDie);
            while (playersToDie.Count > CustomGameOptions.MaxKillsInDetonation) playersToDie.Remove(playersToDie[playersToDie.Count - 1]);
            foreach (var player in playersToDie)
            {
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
        }
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> Shuffle(Il2CppSystem.Collections.Generic.List<PlayerControl> playersToDie)
        {
            var count = playersToDie.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = playersToDie[i];
                playersToDie[i] = playersToDie[r];
                playersToDie[r] = tmp;
            }
            return playersToDie;
        }

        protected override void HudManagerUpdate(HudManager __instance)
        {
            if (PlantButton == null)
            {
                PlantButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                PlantButton.graphic.enabled = true;
                PlantButton.graphic.sprite = TownOfUs.PlantSprite;
                PlantButton.gameObject.SetActive(false);
            }

            PlantButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (Detonating)
            {
                PlantButton.graphic.sprite = TownOfUs.DetonateSprite;
                DetonateTimer();
                PlantButton.SetCoolDown(TimeRemaining, CustomGameOptions.DetonateDelay);
            }
            else
            {
                PlantButton.graphic.sprite = TownOfUs.PlantSprite;
                if (!Detonated) DetonateKillStart();
                if (PlayerControl.LocalPlayer.killTimer > 0)
                {
                    PlantButton.graphic.color = Palette.DisabledClear;
                    PlantButton.graphic.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    PlantButton.graphic.color = Palette.EnabledColor;
                    PlantButton.graphic.material.SetFloat("_Desat", 0f);
                }
                PlantButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer,
                    GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }

            PlantButton.graphic.color = Palette.EnabledColor;
            PlantButton.graphic.material.SetFloat("_Desat", 0f);
            if (PlantButton.graphic.sprite == TownOfUs.PlantSprite)
            {
                PlantButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer,
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }
            else
            {
                PlantButton.SetCoolDown(TimeRemaining, CustomGameOptions.DetonateDelay);
            }
        }

        protected override bool UseKillButton(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (StartTimer() > 0) return false;

            if (__instance == PlantButton)
            {
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (Player.inVent) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (PlantButton.graphic.sprite == TownOfUs.PlantSprite)
                {
                    Detonated = false;
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    pos.z += 0.001f;
                    DetonatePoint = pos;
                    PlantButton.graphic.sprite = TownOfUs.DetonateSprite;
                    TimeRemaining = CustomGameOptions.DetonateDelay;
                    PlantButton.SetCoolDown(TimeRemaining, CustomGameOptions.DetonateDelay);
                    PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    Bomb = pos.CreateBomb();
                    return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}