using HarmonyLib;
using Reactor.Utilities.Extensions;

namespace TownOfUs.CrewmateRoles.AltruistMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var revived in Coroutine.Revived)
            {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead ||
                    revived.Key.Data.IsDead)
                {
                    revived.Value.gameObject.Destroy();
                    Coroutine.Revived.Remove(revived.Key);
                    return;
                }

                revived.Value.target = revived.Key.transform.position;
            }
        }
    }
}