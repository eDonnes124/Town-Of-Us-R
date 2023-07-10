using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Plant
    {
        public static Sprite PlantSprite => TownOfUs.PlantSprite;
        public static Sprite DetonateSprite => TownOfUs.DetonateSprite;
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Bomber);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
            if (role.StartTimer() > 0) return false;

            if (__instance == role.PlantButton)
            {
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.PlantButton.graphic.sprite == PlantSprite)
                {
                    role.Detonated = false;
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    pos.z += 0.001f;
                    role.DetonatePoint = pos;
                    role.PlantButton.graphic.sprite = DetonateSprite;
                    role.TimeRemaining = CustomGameOptions.DetonateDelay;
                    role.PlantButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DetonateDelay);
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                    {
                        var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                        var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay;
                        var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                        PlayerControl.LocalPlayer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                    }
                    else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    role.Bomb = BombExtentions.CreateBomb(pos);
                    return false;
                }
                else return false;
            }
            return true;
        }
    }
}
