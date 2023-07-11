using HarmonyLib;
using TMPro;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.ProsecutorMod
{
    public class AddProsecute
    {
        public static void UpdateButton(Prosecutor role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.Prosecute.gameObject.SetActive(skip.gameObject.active && !role.Prosecuted);
            role.Prosecute.voteComplete = skip.voteComplete;
            role.Prosecute.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            role.Prosecute.GetComponentsInChildren<TextMeshPro>()[0].text = "Prosecute";
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void GenButton(Prosecutor role, MeetingHud __instance)
            {
                var skip = __instance.SkipVoteButton;
                role.Prosecute = Object.Instantiate(skip, skip.transform.parent);
                role.Prosecute.Parent = __instance;
                role.Prosecute.SetTargetPlayerId(251);
                role.Prosecute.transform.localPosition = skip.transform.localPosition +
                                                       new Vector3(0f, -0.17f, 0f);
                skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                UpdateButton(role, __instance);
            }

            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
                var prosRole = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                GenButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
                var prosRole = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
                var prosRole = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                prosRole.Prosecute.ClearButtons();
                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
                var prosRole = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                if (__0 != 251) prosRole.Prosecute.ClearButtons();

                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
                var prosRole = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
                var prosRole = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            prosRole.Prosecute.SetDisabled();
                            break;
                        }


                        prosRole.Prosecute.SetEnabled();
                        break;
                }

                UpdateButton(prosRole, __instance);
            }
        }
    }
}