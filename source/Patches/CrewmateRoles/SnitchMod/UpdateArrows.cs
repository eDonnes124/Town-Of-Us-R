using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateArrows
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
            {
                var snitch = (Snitch)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || snitch.Player.Data.IsDead)
                {
                    snitch.SnitchArrows.Values.DestroyAll();
                    snitch.SnitchArrows.Clear();
                    snitch.ImpArrows.DestroyAll();
                    snitch.ImpArrows.Clear();
                }

                foreach (var arrow in snitch.ImpArrows) arrow.target = snitch.Player.transform.position;

                foreach (var arrow in snitch.SnitchArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);
                    if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        snitch.DestroyArrow(arrow.Key);
                        continue;
                    }
                    arrow.Value.target = player.transform.position;
                }
            }
        }
    }
}