using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TMPro;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using TownOfUs.Patches;
using System.Collections;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.Roles
{
    public class Transporter : Role
    {
        public DateTime LastTransported { get; set; }

        public bool PressedButton;
        public bool MenuClick;
        public bool LastMouse;

        public PoolableBehavior HighlightedPlayer;
        public int PlayerIndex;
        public ChatController TransportList { get; set; }
        public PlayerControl TransportPlayer1 { get; set; }
        public PlayerControl TransportPlayer2 { get; set; }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Dictionary<byte, DateTime> UntransportablePlayers = new Dictionary<byte, DateTime>();

        public Transporter(PlayerControl player) : base(player)
        {
            Name = "Transporter";
            ImpostorText = () => "Choose Two Players To Swap Locations";
            TaskText = () => "Choose two players to swap locations";
            Color = Colors.Transporter;
            LastTransported = DateTime.UtcNow;
            RoleType = RoleEnum.Transporter;
            AddToRoleHistory(RoleType);
            Scale = 1.4f;
            PressedButton = false;
            MenuClick = false;
            LastMouse = false;
            TransportList = null;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            UsesLeft = CustomGameOptions.TransportMaxUses;
        }

        public float TransportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTransported;
            var num = CustomGameOptions.TransportCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Update(HudManager __instance)
        {
            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {
            if (PressedButton && TransportList == null)
            {
                TransportPlayer1 = null;
                TransportPlayer2 = null;


                TransportList = Object.Instantiate(__instance.Chat);
                __instance.Chat.SetVisible(false);

                TransportList.transform.SetParent(Camera.main.transform);
                TransportList.SetVisible(true);
                TransportList.Toggle();

                var aspect = TransportList.gameObject.AddComponent<AspectPosition>();
                aspect.Alignment = AspectPosition.EdgeAlignments.Center;
                aspect.AdjustPosition();

                TransportList.GetPooledBubble().enabled = false;
                TransportList.GetPooledBubble().gameObject.SetActive(false);


                TransportList.freeChatField.enabled = false;
                TransportList.freeChatField.gameObject.SetActive(false);

                TransportList.banButton.MenuButton.enabled = false;
                TransportList.banButton.MenuButton.gameObject.SetActive(false);

                TransportList.freeChatField.charCountText.enabled = false;
                TransportList.freeChatField.charCountText.gameObject.SetActive(false);

                TransportList.openKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                TransportList.openKeyboardButton.Destroy();

                TransportList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>()
                    .enabled = false;
                TransportList.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                TransportList.backgroundImage.enabled = false;

                foreach (var rend in TransportList.chatScreen
                    .GetComponentsInChildren<SpriteRenderer>())
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }

                foreach (var bubble in TransportList.chatBubblePool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                TransportList.chatBubblePool.activeChildren.Clear();

                foreach (var TempPlayer in PlayerControl.AllPlayerControls)
                {
                    if (TempPlayer != null &&
                        !TempPlayer.Data.IsDead &&
                        !TempPlayer.Data.Disconnected &&
                        TempPlayer.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                    {
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (player != null &&
                                ((!player.Data.Disconnected && !player.Data.IsDead) ||
                                Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == player.PlayerId)))
                            {
                                TransportList.AddChat(TempPlayer, "Click here");
                                TransportList.chatBubblePool.activeChildren[TransportList.chatBubblePool.activeChildren._size - 1].Cast<ChatBubble>().SetName(player.Data.PlayerName, false, false,
                                    PlayerControl.LocalPlayer.PlayerId == player.PlayerId ? Color : Color.white);
                                var IsDeadTemp = player.Data.IsDead;
                                player.Data.IsDead = false;
                                TransportList.chatBubblePool.activeChildren[TransportList.chatBubblePool.activeChildren._size - 1].Cast<ChatBubble>().SetCosmetics(player.Data);
                                player.Data.IsDead = IsDeadTemp;
                            }
                        }
                        break;
                    }
                }
            }
            if (TransportList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (!TransportList.IsOpenOrOpening || MeetingHud.Instance || Input.GetKeyInt(KeyCode.Escape) || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    TransportList.Toggle();
                    TransportList.gameObject.SetActive(false);
                    TransportList.gameObject.DestroyImmediate();
                    TransportList = null;
                    PressedButton = false;
                    TransportPlayer1 = null;
                    HighlightedPlayer = null;
                    PlayerIndex = 0;
                }
                else
                {
                    if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU cycle +"))
                    {
                        PlayerIndex = PlayerIndex == TransportList.chatBubblePool.activeChildren.Count - 1 ? 0 : PlayerIndex + 1;
                        HighlightedPlayer = TransportList.chatBubblePool.activeChildren[PlayerIndex];
                    }
                    else if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU cycle -"))
                    {
                        PlayerIndex = PlayerIndex == 0 ? TransportList.chatBubblePool.activeChildren.Count - 1 : PlayerIndex - 1;
                        HighlightedPlayer = TransportList.chatBubblePool.activeChildren[PlayerIndex];
                    }
                    else if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU confirm") && HighlightedPlayer)
                    {
                        CheckClick(__instance, HighlightedPlayer);
                        if (TransportList == null) return;
                    }
                    foreach (var bubble in TransportList.chatBubblePool.activeChildren)
                    {
                        if (bubble == HighlightedPlayer)
                        {
                            bubble.Cast<ChatBubble>().NameText.color = Color.yellow;
                        }
                        else bubble.Cast<ChatBubble>().NameText.color = Color.white;

                        if (TransportTimer() == 0f && TransportList != null)
                        {
                            Vector2 ScreenMin =
                                Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax =
                                Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);
                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x &&
                                Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                            {
                                if (!Input.GetMouseButtonDown(0) && LastMouse)
                                {
                                    LastMouse = false;
                                    CheckClick(__instance, bubble);
                                }
                            }
                        }
                    }
                    if (!Input.GetMouseButtonDown(0) && LastMouse)
                    {
                        if (MenuClick)
                            MenuClick = false;
                        else
                        {
                            TransportList.Toggle();
                            TransportList.SetVisible(false);
                            TransportList = null;
                            PressedButton = false;
                            TransportPlayer1 = null;
                            HighlightedPlayer = null;
                            PlayerIndex = 0;
                        }
                    }
                    LastMouse = Input.GetMouseButtonDown(0);
                }
            }
        }

        private void CheckClick(HudManager __instance, PoolableBehavior bubble)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                {
                    if (TransportPlayer1 == null)
                    {
                        TransportPlayer1 = player;
                        bubble.Cast<ChatBubble>().Background.color = Color.green;
                    }
                    else if (player.PlayerId == TransportPlayer1.PlayerId)
                    {
                        TransportPlayer1 = null;
                        bubble.Cast<ChatBubble>().Background.color = Color.white;
                    }
                    else
                    {
                        PressedButton = false;
                        TransportList.Toggle();
                        TransportList.gameObject.SetActive(false);
                        TransportList = null;

                        TransportPlayer2 = player;

                        HandleMedicPlague(__instance);

                        TransportPlayer1 = null;
                        TransportPlayer2 = null;
                    }
                    if (!Input.GetMouseButtonDown(0) && LastMouse)
                    {
                        if (MenuClick)
                            MenuClick = false;
                        else
                        {
                            TransportList.Toggle();
                            TransportList.SetVisible(false);
                            TransportList = null;
                            PressedButton = false;
                            TransportPlayer1 = null;
                        }
                    }
                    LastMouse = Input.GetMouseButtonDown(0);
                }
            }
        }

        public static IEnumerator TransportPlayers(byte player1, byte player2, bool die)
        {
            var TP1 = Utils.PlayerById(player1);
            var TP2 = Utils.PlayerById(player2);
            var deadBodies = Object.FindObjectsOfType<DeadBody>();
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;
            if (TP1.Data.IsDead)
            {
                foreach (var body in deadBodies) if (body.ParentId == TP1.PlayerId) Player1Body = body;
                if (Player1Body == null) yield break;
            }
            if (TP2.Data.IsDead)
            {
                foreach (var body in deadBodies) if (body.ParentId == TP2.PlayerId) Player2Body = body;
                if (Player2Body == null) yield break;
            }

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                {
                    yield return null;
                }
                TP1.MyPhysics.ExitAllVents();
            }
            if (TP2.inVent && PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                {
                    yield return null;
                }
                TP2.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                var TempFacing = TP1.myRend().flipX;
                TP1.NetTransform.SnapTo(new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f));
                TP1.myRend().flipX = TP2.myRend().flipX;
                if (die) Utils.MurderPlayer(TP1, TP2, true);
                else
                {
                    TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));
                    TP2.myRend().flipX = TempFacing;
                }

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }

            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TP2.GetTruePosition();
                TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }

                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                StopDragging(Player2Body.ParentId);
                TP1.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                TP1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;
                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                StopDragging(Player2Body.ParentId);
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = Player2Body.TruePosition;
                Player2Body.transform.position = TempPosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId ||
                PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Transporter));
                if (Minigame.Instance) Minigame.Instance.Close();
            }

            TP1.moveable = true;
            TP2.moveable = true;
            TP1.Collider.enabled = true;
            TP2.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.NetTransform.enabled = true;
        }

        public static void StopDragging(byte PlayerId)
        {
            var Undertaker = (Undertaker)Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Undertaker);
            if (Undertaker != null && Undertaker.CurrentlyDragging != null &&
                Undertaker.CurrentlyDragging.ParentId == PlayerId)
                Undertaker.CurrentlyDragging = null;
        }

        public void HandleMedicPlague(HudManager __instance)
        {
            if (!UntransportablePlayers.ContainsKey(TransportPlayer1.PlayerId) && !UntransportablePlayers.ContainsKey(TransportPlayer2.PlayerId))
            {
                if (Player.IsInfected() || TransportPlayer1.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(Player, TransportPlayer1);
                }
                if (Player.IsInfected() || TransportPlayer2.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(Player, TransportPlayer2);
                }
                var role = GetRole(Player);
                var transRole = (Transporter)role;
                if (TransportPlayer1.Is(RoleEnum.Pestilence) || TransportPlayer1.IsOnAlert())
                {
                    if (Player.IsShielded())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, Player.GetMedic().Player.PlayerId, Player.PlayerId);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        if (CustomGameOptions.ShieldBreaks)
                            transRole.LastTransported = DateTime.UtcNow;
                        StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        return;
                    }
                    else if (!Player.IsProtected())
                    {
                        Coroutines.Start(TransportPlayers(TransportPlayer1.PlayerId, Player.PlayerId, true));

                        Utils.Rpc(CustomRPC.Transport, TransportPlayer1.PlayerId, Player.PlayerId, true);
                        return;
                    }
                    transRole.LastTransported = DateTime.UtcNow;
                    return;
                }
                else if (TransportPlayer2.Is(RoleEnum.Pestilence) || TransportPlayer2.IsOnAlert())
                {
                    if (Player.IsShielded())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, Player.GetMedic().Player.PlayerId, Player.PlayerId);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        if (CustomGameOptions.ShieldBreaks)
                            transRole.LastTransported = DateTime.UtcNow;
                        StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        return;
                    }
                    else if (!Player.IsProtected())
                    {
                        Coroutines.Start(TransportPlayers(TransportPlayer2.PlayerId, Player.PlayerId, true));

                        Utils.Rpc(CustomRPC.Transport, TransportPlayer2.PlayerId, Player.PlayerId, true);
                        return;
                    }
                    transRole.LastTransported = DateTime.UtcNow;
                    return;
                }
                LastTransported = DateTime.UtcNow;
                UsesLeft--;

                Coroutines.Start(TransportPlayers(TransportPlayer1.PlayerId, TransportPlayer2.PlayerId, false));

                Utils.Rpc(CustomRPC.Transport, TransportPlayer1.PlayerId, TransportPlayer2.PlayerId, false);
            }
            else
            {
                __instance.StartCoroutine(Effects.SwayX(__instance.KillButton.transform));
            }
        }
    }
}