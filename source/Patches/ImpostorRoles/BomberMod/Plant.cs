using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class Plant
    {
        public static bool Prefix(AbilityButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Bomber);
            if (!flag) return true;
            if (RoleManager.IsGhostRole(PlayerControl.LocalPlayer.Data.RoleType)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
            if (role.StartTimer() > 0) return false;

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (role.Player.inVent) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (__instance.graphic.sprite == TownOfUs.PlantSprite)
            {
                role.Detonated = false;
                var pos = PlayerControl.LocalPlayer.transform.position;
                pos.z += 0.001f;
                role.DetonatePoint = pos;
                __instance.graphic.sprite = TownOfUs.DetonateSprite;
                role.TimeRemaining = CustomGameOptions.DetonateDelay;
                __instance.SetCoolDown(role.TimeRemaining, CustomGameOptions.DetonateDelay);
                PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                role.Bomb = pos.CreateBomb();
                return false;
            }
            else return false;
        }
    }
}
