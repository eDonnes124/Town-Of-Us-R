using System.Linq;
using HarmonyLib;
using Reactor;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Snitch)) return;
            if (__instance.Data.IsDead) return;
            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var role = Role.GetRole<Snitch>(__instance);
            var localRole = Role.GetRole(PlayerControl.LocalPlayer);
            switch (tasksLeft)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    if (tasksLeft == CustomGameOptions.SnitchTasksRemaining)
                    {
                        role.RegenTask();
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                        {
                            Coroutines.Start(Utils.FlashCoroutine(role.Color));
                        }
                        else if (PlayerControl.LocalPlayer.Data.IsImpostor() || ((PlayerControl.LocalPlayer.Is(RoleEnum.Glitch) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)) && CustomGameOptions.SnitchSeesNeutrals))
                        {
                            Coroutines.Start(Utils.FlashCoroutine(role.Color));
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Sprite;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            role.ImpArrows.Add(arrow);
                        }
                    }
                    break;

                case 0:
                    role.RegenTask();
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsImpostor());
                        foreach (var imp in impostors)
                        {
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Sprite;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            role.SnitchArrows.Add(imp.PlayerId, arrow);
                        }
                    }
                    else if (PlayerControl.LocalPlayer.Data.IsImpostor() || ((PlayerControl.LocalPlayer.Is(RoleEnum.Glitch) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)) && CustomGameOptions.SnitchSeesNeutrals))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    }

                    break;
            }
        }
    }
}