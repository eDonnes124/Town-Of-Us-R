using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;

namespace TownOfUs.CultistRoles.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance != PlayerControl.LocalPlayer) return;
            if (!__instance.Is(RoleEnum.CultistSnitch)) return;
            var role = Role.GetRole<CultistSnitch>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == 0)
            {
                role.CompletedTasks = true;
                if (PlayerControl.LocalPlayer == role.Player)
                {
                    if (role.Player.Is(Faction.Impostors))
                    {
                        var crew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor)).ToList();
                        crew.Shuffle();
                        role.RevealedPlayer = crew[0];
                    }
                    else
                    {
                        var imps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Is(RoleEnum.Necromancer) && !x.Is(RoleEnum.Whisperer)).ToList();
                        if (imps.Count == 0) imps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Necromancer) || x.Is(RoleEnum.Whisperer)).ToList();
                        imps.Shuffle();
                        role.RevealedPlayer = imps[0];
                    }
                    Utils.Rpc(CustomRPC.SnitchCultistReveal, role.Player.PlayerId, role.RevealedPlayer.PlayerId);
                }
            }
        }
    }
}