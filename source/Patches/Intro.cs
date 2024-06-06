using HarmonyLib;
using System;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using TownOfUs.Roles;
using AmongUs.GameOptions;

namespace TownOfUs.Patches
{
        [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
        public static class IntroCutScenePatch
        {
                public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
                {
                    var role = Role.GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (role.Faction == Faction.NeutralKilling || role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = $"Neutral\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = Role.GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        if (role.Faction == Faction.Crewmates)
                        {
                            __instance.__4__this.TeamTitle.text = $"Crewmate\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Palette.CrewmateBlue;
                        }
                        if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = $"Impostor\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Impostor;
                        }
                        var GetFirstLetter = role.Name.Remove(role.Name.Length - (role.Name.Length - 1));
                        if (role.RoleType != RoleEnum.Glitch)
                        {
                        if (CustomGameOptions.GameMode == GameMode.Classic)
                        __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>The " + role.Name + "</size>")));
                        else if ((GetFirstLetter == "A") || (GetFirstLetter == "E") || (GetFirstLetter == "I") || (GetFirstLetter == "U") || (GetFirstLetter == "O"))
                        __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>An " + role.Name + "</size>")));
                        else __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>A " + role.Name + "</size>")));
                        }
                        __instance.__4__this.RoleText.text = "";
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                    }

                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                    {
                        if (modifier.GetType() == typeof(Lover))
                        __instance.__4__this.RoleText.text = $"<size=50%>{modifier.TaskText()}</size>";
                        else
                        __instance.__4__this.RoleText.text = $"<size=50%>You are also {modifier.Name}</size>";
                        __instance.__4__this.RoleText.color = modifier.Color;
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
            public static class IntroCutscene_ShowRole_d__24
            {
                public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
                {
                    var role = Role.GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (role.Faction == Faction.NeutralKilling || role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = $"Neutral\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = Role.GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        if (role.Faction == Faction.Crewmates)
                        {
                            __instance.__4__this.TeamTitle.text = $"Crewmate\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Palette.CrewmateBlue;
                        }
                        if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = $"Impostor\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Impostor;
                        }
                        var GetFirstLetter = role.Name.Remove(role.Name.Length - (role.Name.Length - 1));
                        if (role.RoleType != RoleEnum.Glitch)
                        {
                        if (CustomGameOptions.GameMode == GameMode.Classic)
                        __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>The " + role.Name + "</size>")));
                        else if ((GetFirstLetter == "A") || (GetFirstLetter == "E") || (GetFirstLetter == "I") || (GetFirstLetter == "U") || (GetFirstLetter == "O"))
                        __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>An " + role.Name + "</size>")));
                        else __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>A " + role.Name + "</size>")));
                        }
                        __instance.__4__this.RoleText.text = "";
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                    }

                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                    {
                        if (modifier.GetType() == typeof(Lover))
                        __instance.__4__this.RoleText.text = $"<size=50%>{modifier.TaskText()}</size>";
                        else
                        __instance.__4__this.RoleText.text = $"<size=50%>You are also {modifier.Name}</size>";
                        __instance.__4__this.RoleText.color = modifier.Color;
                    }

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
            public static class IntroCutscene_CoBegin_d__29
            {
                public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
                {
                    var role = Role.GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (role.Faction == Faction.NeutralKilling || role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = $"Neutral\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = Role.GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        if (role.Faction == Faction.Crewmates)
                        {
                            __instance.__4__this.TeamTitle.text = $"Crewmate\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Palette.CrewmateBlue;
                        }
                        if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = $"Impostor\n<size=30%>Your Role Is...";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Impostor;
                        }
                        var GetFirstLetter = role.Name.Remove(role.Name.Length - (role.Name.Length - 1));
                        if (role.RoleType != RoleEnum.Glitch)
                        {
                        if (CustomGameOptions.GameMode == GameMode.Classic)
                        __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>The " + role.Name + "</size>")));
                        else if ((GetFirstLetter == "A") || (GetFirstLetter == "E") || (GetFirstLetter == "I") || (GetFirstLetter == "U") || (GetFirstLetter == "O"))
                        __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>An " + role.Name + "</size>")));
                        else __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "<size=125%>A " + role.Name + "</size>")));
                        }
                        __instance.__4__this.RoleText.text = "";
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                    }

                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                    {
                        if (modifier.GetType() == typeof(Lover))
                        __instance.__4__this.RoleText.text = $"<size=50%>{modifier.TaskText()}</size>";
                        else
                        __instance.__4__this.RoleText.text = $"<size=50%>You are also {modifier.Name}</size>";
                        __instance.__4__this.RoleText.color = modifier.Color;
                    }

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                }
            }
        }