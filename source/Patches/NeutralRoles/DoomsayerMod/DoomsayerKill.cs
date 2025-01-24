using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.ImpostorRoles.BlackmailerMod;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.Patches;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.DeputyMod;

namespace TownOfUs.NeutralRoles.DoomsayerMod
{
    public class DoomsayerKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl doomsayer)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, doomsayer);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl doomsayer)
        {
            DoomKillCount(player, doomsayer);
            MurderPlayer(voteArea, player);
            Utils.Rpc(CustomRPC.DoomsayerKill, player.PlayerId, doomsayer.PlayerId);
        }

        public static void MurderPlayer(PlayerControl player, bool checkLover = true, bool showKillAnim = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(voteArea, player, checkLover, showKillAnim);
        }
        public static void DoomKillCount(PlayerControl player, PlayerControl doomsayer)
        {
            var doom = Role.GetRole<Doomsayer>(doomsayer);
            doom.CorrectAssassinKills += 1;
            doom.WonByGuessing = true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) MurderPlayer(doom.Player, true, false);
        }
        public static void MurderPlayer(
            PlayerVoteArea voteArea,
            PlayerControl player,
            bool checkLover = true,
            bool showKillAnim = true
        )
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            if (showKillAnim)
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
                    for (int i = 0;i < player.myTasks.Count;i++)
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
                    ShowHideButtonsDoom.HideTextDoom(doomsayer);
                }

                if (player.Is(RoleEnum.Deputy))
                {
                    var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                    RemoveButtons.HideButtons(dep);
                }

                if (player.Is(RoleEnum.Politician))
                {
                    var politician = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
                    politician.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.President))
                {
                    var president = Role.GetRole<President>(PlayerControl.LocalPlayer);
                    president.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.Jailor))
                {
                    var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                    jailor.ExecuteButton.Destroy();
                    jailor.UsesText.Destroy();
                }

                if (player.Is(RoleEnum.Hypnotist))
                {
                    var hypnotist = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
                    hypnotist.HysteriaButton.Destroy();
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Modifier.GetModifier<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence)) MurderPlayer(otherLover, false, false);
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

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (dep.Buttons.Count > 0 && dep.Buttons[voteArea.TargetPlayerId] != null)
                {
                    dep.Buttons[voteArea.TargetPlayerId].SetActive(false);
                    dep.Buttons[voteArea.TargetPlayerId].GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                var index = int.MaxValue;
                for (var i = 0; i < swapper.ListOfActives.Count; i++)
                {
                    if (swapper.ListOfActives[i].Item1 == voteArea.TargetPlayerId)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != int.MaxValue)
                {
                    var button = swapper.Buttons[index];
                    if (button != null)
                    {
                        if (button.GetComponent<SpriteRenderer>().sprite == TownOfUs.SwapperSwitch)
                        {
                            swapper.ListOfActives[index] = (swapper.ListOfActives[index].Item1, false);
                            if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                            if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                            Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                        }
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                        swapper.Buttons[index] = null;
                    }
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
                if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results && MeetingHud.Instance.state != MeetingHud.VoteStates.Proceeding)
                {
                    AddButtonImitator.GenButton(imitatorRole, voteArea, true);
                }
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }
    }
}
