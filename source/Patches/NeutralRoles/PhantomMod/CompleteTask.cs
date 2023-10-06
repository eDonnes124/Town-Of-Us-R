using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            var role = Role.GetRole<Phantom>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
                if (AmongUsClient.Instance.AmHost)
                {
                    Utils.Rpc(CustomRPC.PhantomWin, role.Player.PlayerId);
                    if (CustomGameOptions.NeutralEvilWinEndsGame) Utils.EndGame();
                    else
                    {
                        role.Caught = true;
                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) || !CustomGameOptions.PhantomSpook) return;
                        byte[] toKill = MeetingHud.Instance.playerStates.Where(x => !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.Pestilence)).Select(x => x.TargetPlayerId).ToArray();
                        role.PauseEndCrit = true;
                        var pk = new PunishmentKill((x) => {
                            Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, x);
                            role.PauseEndCrit = false;
                        }, (y) => {
                            return toKill.Contains(y.PlayerId);
                        });
                        Coroutines.Start(pk.Open(1f));
                    }
                }
            }
        }
    }
}