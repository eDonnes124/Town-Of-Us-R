using System;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.ImpostorRoles.BlackmailerMod;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.ImitatorMod;
using Reactor.Utilities;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.NeutralRoles.DoomsayerMod;

namespace TownOfUs.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddJailButtons
    {
        public static Sprite CellSprite => TownOfUs.InJailSprite;
        public static Sprite ExecuteSprite => TownOfUs.ExecuteSprite;

        public static void GenCell(Jailor role, PlayerVoteArea voteArea)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;

            var jailCell = Object.Instantiate(confirmButton, voteArea.transform);
            var cellRenderer = jailCell.GetComponent<SpriteRenderer>();
            var passive = jailCell.GetComponent<PassiveButton>();
            cellRenderer.sprite = CellSprite;
            jailCell.transform.localPosition = new Vector3(-0.95f, 0f, -2f);
            jailCell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            jailCell.layer = 5;
            jailCell.transform.parent = parent;
            jailCell.transform.GetChild(0).gameObject.Destroy();

            passive.OnClick = new Button.ButtonClickedEvent();
            role.JailCell = jailCell;
        }

        public static void GenButton(Jailor role, int index)
        {
            var voteArea = MeetingHud.Instance.playerStates[index];
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = ExecuteSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Execute(role));
            role.ExecuteButton = newButton;

            var usesText = Object.Instantiate(voteArea.NameText, voteArea.transform);
            usesText.transform.localPosition = new Vector3(-0.22f, 0.12f, 0f);
            usesText.text = role.Executes + "";
            usesText.transform.localScale = usesText.transform.localScale * 0.65f;
            role.UsesText = usesText;
        }


        private static Action Execute(Jailor role)
        {
            void Listener()
            {
                role.ExecuteButton.Destroy();
                role.UsesText.Destroy();
                role.JailCell.Destroy();
                role.Executes -= 1;
                if (!role.Jailed.Is(RoleEnum.Pestilence))
                {
                    if (role.Jailed.Is(Faction.Crewmates))
                    {
                        role.IncorrectKills += 1;
                        role.CanJail = false;
                        role.Executes = 0;
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    }
                    else
                    {
                        role.CorrectKills += 1;
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    }
                    ExecuteKill(role, role.Jailed);
                    Utils.Rpc(CustomRPC.Jail, role.Player.PlayerId, (byte)1);
                    role.Jailed = null;
                }
            }

            return Listener;
        }

        public static void ExecuteKill (Jailor jailor, PlayerControl player, bool checkLover = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );

            var hudManager = DestroyableSingleton<HudManager>.Instance;
            if (checkLover)
            {
                SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
                hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
            }
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                Utils.ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);

                if (!checkLover)
                {

                    if (player.Is(RoleEnum.Swapper))
                    {
                        var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                        var buttons = Role.GetRole<Swapper>(player).Buttons;
                        foreach (var button in buttons)
                        {
                            if (button != null)
                            {
                                button.SetActive(false);
                                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                            }
                        }
                        swapper.ListOfActives.Clear();
                        swapper.Buttons.Clear();
                        SwapVotes.Swap1 = null;
                        SwapVotes.Swap2 = null;
                        Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    }

                    if (player.Is(RoleEnum.Imitator))
                    {
                        var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                        var buttons = Role.GetRole<Imitator>(player).Buttons;
                        foreach (var button in buttons)
                        {
                            if (button != null)
                            {
                                button.SetActive(false);
                                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                            }
                        }
                        imitator.ListOfActives.Clear();
                        imitator.Buttons.Clear();
                        SetImitate.Imitate = null;
                    }

                    if (player.Is(RoleEnum.Vigilante))
                    {
                        var retributionist = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                        ShowHideButtonsVigi.HideButtonsVigi(retributionist);
                    }

                    if (player.Is(AbilityEnum.Assassin))
                    {
                        var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                        ShowHideButtons.HideButtons(assassin);
                    }

                    if (player.Is(RoleEnum.Doomsayer))
                    {
                        var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                        ShowHideButtonsDoom.HideButtonsDoom(doomsayer);
                    }

                    if (player.Is(RoleEnum.Politician))
                    {
                        var politician = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
                        politician.RevealButton.Destroy();
                    }

                    if (player.Is(RoleEnum.Mayor))
                    {
                        var mayor = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                        mayor.RevealButton.Destroy();
                    }

                    if (player.Is(RoleEnum.Jailor))
                    {
                        var jailor2 = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                        jailor2.ExecuteButton.Destroy();
                        jailor2.UsesText.Destroy();
                    }

                    if (player.Is(RoleEnum.Hypnotist))
                    {
                        var hypnotist = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
                        hypnotist.HysteriaButton.Destroy();
                    }
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Modifier.GetModifier<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence)) ExecuteKill(jailor, otherLover, false);
            }

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);
            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();
            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = MeetingHud.Instance;
            if (amOwner)
            {
                meetingHud.SetForegroundForDead();
            }

            var blackmailers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();
            foreach (var role in blackmailers)
            {
                if (role.Blackmailed != null && voteArea.TargetPlayerId == role.Blackmailed.PlayerId)
                {
                    if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(
                            voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                            voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset,
                            voteArea.XMark.transform.localPosition.z);
                    }
                }
            }

            if (!checkLover)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var vigi = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                    ShowHideButtonsVigi.HideTarget(vigi, voteArea.TargetPlayerId);
                }

                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                    ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var doom = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                    ShowHideButtonsDoom.HideTarget(doom, voteArea.TargetPlayerId);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    var button = swapper.Buttons[voteArea.TargetPlayerId];
                    if (button.GetComponent<SpriteRenderer>().sprite == TownOfUs.SwapperSwitch)
                    {
                        swapper.ListOfActives[voteArea.TargetPlayerId] = false;
                        if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                        if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                        Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    }
                    button.SetActive(false);
                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    swapper.Buttons[voteArea.TargetPlayerId] = null;
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester))
            {
                var jester = (Jester)Role.GetRole(PlayerControl.LocalPlayer);
                if (jester.canKill && jester.KillableVoters.Contains(voteArea.TargetPlayerId) && jester.meetingButtons.ContainsKey(voteArea.TargetPlayerId))
                {
                    jester.meetingButtons[voteArea.TargetPlayerId].Destroy();
                    jester.meetingButtons.Remove(voteArea.TargetPlayerId);
                }
            }

            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (playerVoteArea.VotedFor != player.PlayerId) continue;
                playerVoteArea.UnsetVote();
                var voteAreaPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);
                if (voteAreaPlayer.Is(RoleEnum.Prosecutor))
                {
                    var pros = Role.GetRole<Prosecutor>(voteAreaPlayer);
                    pros.ProsecuteThisMeeting = false;
                }
                if (!voteAreaPlayer.AmOwner) continue;
                meetingHud.ClearVote();
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Imitator) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var imitatorRole = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                if (!meetingHud.playerStates[PlayerControl.LocalPlayer.PlayerId].DidVote)
                {
                    RoleEnum imitatedRole = Role.GetRole(player).RoleType;
                    var imitatable = imitatorRole.ImitatableRoles.Contains(imitatedRole);
                    AddButtonImitator.GenButton(imitatorRole, player.PlayerId, imitatable, true);
                }
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Jailor))
            {
                var jailor = (Jailor)role;
                jailor.JailCell.Destroy();
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
                if (jailor.Jailed == null) return;
                if (jailor.Player.Data.IsDead || jailor.Player.Data.Disconnected) return;
                if (jailor.Jailed.Data.IsDead || jailor.Jailed.Data.Disconnected) return;
                foreach (var voteArea in __instance.playerStates)
                    if (jailor.Jailed.PlayerId == voteArea.TargetPlayerId)
                    {
                        GenCell(jailor, voteArea);
                    }
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
            var jailorRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
            if (jailorRole.Executes <= 0 || jailorRole.Jailed.Data.IsDead || jailorRole.Jailed.Data.Disconnected) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (jailorRole.Jailed.PlayerId == __instance.playerStates[i].TargetPlayerId)
                {
                    if (!(jailorRole.Jailed.IsLover() && PlayerControl.LocalPlayer.IsLover())) GenButton(jailorRole, i);
                }
        }
    }
}