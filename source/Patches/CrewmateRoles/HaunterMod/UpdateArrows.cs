using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Haunter))
            {
                var haunter = (Haunter)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || haunter.Caught)
                {
                    haunter.HaunterArrows.DestroyAll();
                    haunter.HaunterArrows.Clear();
                    haunter.ImpArrows.DestroyAll();
                    haunter.ImpArrows.Clear();
                }

                foreach (var arrow in haunter.ImpArrows) arrow.target = haunter.Player.transform.position;

                foreach (var (arrow, target) in Utils.Zip(haunter.HaunterArrows, haunter.HaunterTargets))
                {
                    if (target.Data.IsDead)
                    {
                        arrow.Destroy();
                        if (arrow.gameObject != null) arrow.gameObject.Destroy();
                    }

                    arrow.target = target.transform.position;
                }
            }
        }
    }
}