using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {

        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);
            try
            {
                PlayerMenu.singleton.Menu.Close();
            }
            catch
            {
                var pk = new PlayerMenu((x) =>
                {
                    if(!role.HasKilled) Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, x);
                    role.HasKilled = true;
                }, (y) =>
                {
                    return role.KillableVoters.Contains(y.PlayerId);
                });

                Coroutines.Start(pk.Open(0f));
            }

            return false;
        }
    }
}
