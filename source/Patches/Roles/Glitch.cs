using Hazel;
using InnerNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reactor;
using TownOfUs.CrewmateRoles.MedicMod;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using HarmonyLib;

namespace TownOfUs.Roles
{
    public class Glitch : Role, IVisualAlteration
    {
        public static AssetBundle bundle = loadBundle();
        public static Sprite MimicSprite = bundle.LoadAsset<Sprite>("MimicSprite").DontUnload();
        public static Sprite HackSprite = bundle.LoadAsset<Sprite>("HackSprite").DontUnload();
        public static Sprite LockSprite = bundle.LoadAsset<Sprite>("Lock").DontUnload();

        public bool lastMouse;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "The Glitch";
            Color = Patches.Colors.Glitch;
            LastHack = DateTime.UtcNow;
            LastMimic = DateTime.UtcNow;
            LastKill = DateTime.UtcNow;
            HackButton = null;
            MimicButton = null;
            KillTarget = null;
            HackTarget = null;
            MimicList = null;
            IsUsingMimic = false;
            RoleType = RoleEnum.Glitch;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "You are the glitch";
            TaskText = () => "Murder players as the Glitch:";
            Faction = Faction.Neutral;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastMimic { get; set; }
        public DateTime LastHack { get; set; }
        public DateTime LastKill { get; set; }
        public KillButton HackButton { get; set; }
        public KillButton MimicButton { get; set; }
        public PlayerControl KillTarget { get; set; }
        public PlayerControl HackTarget { get; set; }
        public ChatController MimicList { get; set; }
        public bool IsUsingMimic { get; set; }

        public PlayerControl MimicTarget { get; set; }
        public bool GlitchWins { get; set; }

        public static AssetBundle loadBundle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("TownOfUs.Resources.glitchbundle");
            var assets = stream.ReadFully();
            return AssetBundle.LoadFromMemory(assets);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 1)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.GlitchWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Glitch Edition");
            GlitchWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            var glitchTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            glitchTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = glitchTeam;
        }

        public void Update(HudManager __instance)
        {
            if (!Player.Data.IsDead)
            {
                Utils.SetClosestPlayer(ref ClosestPlayer);
            }

            Player.nameText.color = Color;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Player.PlayerId == player.TargetPlayerId)
                        player.NameText.color = Color;

            if (HudManager.Instance != null && HudManager.Instance.Chat != null)
                foreach (var bubble in HudManager.Instance.Chat.chatBubPool.activeChildren)
                    if (bubble.Cast<ChatBubble>().NameText != null &&
                        Player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                        bubble.Cast<ChatBubble>().NameText.color = Color;

            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {
            KillButtonHandler.KillButtonUpdate(this, __instance);

            MimicButtonHandler.MimicButtonUpdate(this, __instance);

            HackButtonHandler.HackButtonUpdate(this, __instance);

            if (__instance.KillButton != null && Player.Data.IsDead)
                __instance.KillButton.SetTarget(null);

            if (MimicButton != null && Player.Data.IsDead)
                MimicButton.SetTarget(null);

            if (HackButton != null && Player.Data.IsDead)
                HackButton.SetTarget(null);

            if (MimicList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (!MimicList.IsOpen || MeetingHud.Instance)
                {
                    MimicList.Toggle();
                    MimicList.SetVisible(false);
                    MimicList = null;
                }
                else
                {
                    foreach (var bubble in MimicList.chatBubPool.activeChildren)
                        if (!IsUsingMimic && MimicList != null)
                        {
                            Vector2 ScreenMin =
                                Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax =
                                Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);
                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x)
                                if (Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                                {
                                    if (!Input.GetMouseButtonDown(0) && lastMouse)
                                    {
                                        lastMouse = false;
                                        MimicList.Toggle();
                                        MimicList.SetVisible(false);
                                        MimicList = null;
                                        RpcSetMimicked(PlayerControl.AllPlayerControls.ToArray().Where(x =>
                                                x.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                                            .FirstOrDefault());
                                        break;
                                    }

                                    lastMouse = Input.GetMouseButtonDown(0);
                                }
                        }
                }
            }
        }

        public bool UseAbility(KillButton __instance)
        {
            if (__instance == HackButton)
                HackButtonHandler.HackButtonPress(this, __instance);
            else if (__instance == MimicButton)
                MimicButtonHandler.MimicButtonPress(this, __instance);
            else
                KillButtonHandler.KillButtonPress(this, __instance);

            return false;
        }

        public void RpcSetHacked(PlayerControl hacked)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetHacked, SendOption.Reliable, -1);
            writer.Write(hacked.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            SetHacked(hacked);
        }

        public void SetHacked(PlayerControl hacked)
        {
            LastHack = DateTime.UtcNow;
            Coroutines.Start(AbilityCoroutine.Hack(this, hacked));
        }

        public void RpcSetMimicked(PlayerControl mimicked)
        {
            Coroutines.Start(AbilityCoroutine.Mimic(this, mimicked));
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (IsUsingMimic)
            {
                appearance = MimicTarget.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MimicTarget);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public static class AbilityCoroutine
        {
            public static Dictionary<byte, DateTime> tickDictionary = new Dictionary<byte, DateTime>();

            public static IEnumerator Hack(Glitch __instance, PlayerControl hackPlayer)
            {
                GameObject[] lockImg = { null, null, null, null };
                ImportantTextTask hackText;

                if (tickDictionary.ContainsKey(hackPlayer.PlayerId))
                {
                    tickDictionary[hackPlayer.PlayerId] = DateTime.UtcNow;
                    yield break;
                }

                hackText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                hackText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                hackText.Text =
                    $"{__instance.ColorString}Hacked {hackPlayer.Data.PlayerName} ({CustomGameOptions.HackDuration}s)</color>";
                hackText.Index = hackPlayer.PlayerId;
                tickDictionary.Add(hackPlayer.PlayerId, DateTime.UtcNow);
                PlayerControl.LocalPlayer.myTasks.Insert(0, hackText);

                while (true)
                {
                    if (PlayerControl.LocalPlayer == hackPlayer)
                    {
                        if (HudManager.Instance.KillButton != null)
                        {
                            if (lockImg[0] == null)
                            {
                                lockImg[0] = new GameObject();
                                var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[0].layer = 5;
                            lockImg[0].transform.position =
                                new Vector3(HudManager.Instance.KillButton.transform.position.x,
                                    HudManager.Instance.KillButton.transform.position.y, -50f);
                            HudManager.Instance.KillButton.enabled = false;
                            HudManager.Instance.KillButton.graphic.color = Palette.DisabledClear;
                            HudManager.Instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                        }

                        if (HudManager.Instance.UseButton != null)
                        {
                            if (lockImg[1] == null)
                            {
                                lockImg[1] = new GameObject();
                                var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[1].transform.position =
                                new Vector3(HudManager.Instance.UseButton.transform.position.x,
                                    HudManager.Instance.UseButton.transform.position.y, -50f);
                            lockImg[1].layer = 5;
                            HudManager.Instance.UseButton.enabled = false;
                            HudManager.Instance.UseButton.graphic.color = Palette.DisabledClear;
                            HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 1f);
                        }

                        if (HudManager.Instance.ReportButton != null)
                        {
                            if (lockImg[2] == null)
                            {
                                lockImg[2] = new GameObject();
                                var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[2].transform.position =
                                new Vector3(HudManager.Instance.ReportButton.transform.position.x,
                                    HudManager.Instance.ReportButton.transform.position.y, -50f);
                            lockImg[2].layer = 5;
                            HudManager.Instance.ReportButton.enabled = false;
                            HudManager.Instance.ReportButton.SetActive(false);
                        }

                        var role = GetRole(PlayerControl.LocalPlayer);
                        if (role != null)
                            if (role.ExtraButtons.Count > 0)
                            {
                                if (lockImg[3] == null)
                                {
                                    lockImg[3] = new GameObject();
                                    var lockImgR = lockImg[3].AddComponent<SpriteRenderer>();
                                    lockImgR.sprite = LockSprite;
                                }

                                lockImg[3].transform.position = new Vector3(
                                    role.ExtraButtons[0].transform.position.x,
                                    role.ExtraButtons[0].transform.position.y, -50f);
                                lockImg[3].layer = 5;
                                role.ExtraButtons[0].enabled = false;
                                role.ExtraButtons[0].graphic.color = Palette.DisabledClear;
                                role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 1f);
                            }

                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }

                    var totalHacktime = (DateTime.UtcNow - tickDictionary[hackPlayer.PlayerId]).TotalMilliseconds /
                                        1000;
                    hackText.Text =
                        $"{__instance.ColorString}Hacked {hackPlayer.Data.PlayerName} ({CustomGameOptions.HackDuration - Math.Round(totalHacktime)}s)</color>";
                    if (MeetingHud.Instance || totalHacktime > CustomGameOptions.HackDuration || hackPlayer == null ||
                        hackPlayer.Data.IsDead)
                    {
                        foreach (var obj in lockImg)
                            if (obj != null)
                                obj.SetActive(false);

                        if (PlayerControl.LocalPlayer == hackPlayer)
                        {
                            HudManager.Instance.UseButton.enabled = true;
                            HudManager.Instance.ReportButton.enabled = true;
                            HudManager.Instance.KillButton.enabled = true;
                            HudManager.Instance.UseButton.graphic.color = Palette.EnabledColor;
                            HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 0f);
                            var role = GetRole(PlayerControl.LocalPlayer);
                            if (role != null)
                                if (role.ExtraButtons.Count > 0)
                                {
                                    role.ExtraButtons[0].enabled = true;
                                    role.ExtraButtons[0].graphic.color = Palette.EnabledColor;
                                    role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 0f);
                                }
                        }

                        tickDictionary.Remove(hackPlayer.PlayerId);
                        PlayerControl.LocalPlayer.myTasks.Remove(hackText);
                        yield break;
                    }

                    yield return null;
                }
            }

            public static IEnumerator Mimic(Glitch __instance, PlayerControl mimicPlayer)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetMimic, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(mimicPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Utils.Morph(__instance.Player, mimicPlayer, true);

                var mimicActivation = DateTime.UtcNow;
                var mimicText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                mimicText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                mimicText.Text =
                    $"{__instance.ColorString}Mimicking {mimicPlayer.Data.PlayerName} ({CustomGameOptions.MimicDuration}s)</color>";
                PlayerControl.LocalPlayer.myTasks.Insert(0, mimicText);

                while (true)
                {
                    __instance.IsUsingMimic = true;
                    __instance.MimicTarget = mimicPlayer;
                    var totalMimickTime = (DateTime.UtcNow - mimicActivation).TotalMilliseconds / 1000;
                    if (__instance.Player.Data.IsDead)
                    {
                        totalMimickTime = CustomGameOptions.MimicDuration;
                    }
                    mimicText.Text =
                        $"{__instance.ColorString}Mimicking {mimicPlayer.Data.PlayerName} ({CustomGameOptions.MimicDuration - Math.Round(totalMimickTime)}s)</color>";
                    if (totalMimickTime > CustomGameOptions.MimicDuration ||
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
                    {
                        PlayerControl.LocalPlayer.myTasks.Remove(mimicText);
                        //System.Console.WriteLine("Unsetting mimic");
                        __instance.LastMimic = DateTime.UtcNow;
                        __instance.IsUsingMimic = false;
                        __instance.MimicTarget = null;
                        Utils.Unmorph(__instance.Player);

                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(
                            PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RpcResetAnim, SendOption.Reliable,
                            -1);
                        writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer2.Write(mimicPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        yield break;
                    }

                    Utils.Morph(__instance.Player, mimicPlayer);
                    __instance.MimicButton.SetCoolDown(CustomGameOptions.MimicDuration - (float)totalMimickTime,
                        CustomGameOptions.MimicDuration);

                    yield return null;
                }
            }
        }

        public static class KillButtonHandler
        {
            public static void KillButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (!__gInstance.Player.Data.IsImpostor() && Input.GetKeyDown(KeyCode.Q))
                    __instance.KillButton.DoClick();

                __instance.KillButton.gameObject.SetActive(__instance.UseButton.isActiveAndEnabled && !MeetingHud.Instance &&
                                                           !__gInstance.Player.Data.IsDead);
                __instance.KillButton.SetCoolDown(
                    CustomGameOptions.GlitchKillCooldown -
                    (float)(DateTime.UtcNow - __gInstance.LastKill).TotalSeconds,
                    CustomGameOptions.GlitchKillCooldown);

                __instance.KillButton.SetTarget(null);
                __gInstance.KillTarget = null;

                if (__instance.KillButton.isActiveAndEnabled)
                {
                    __instance.KillButton.SetTarget(__gInstance.ClosestPlayer);
                    __gInstance.KillTarget = __gInstance.ClosestPlayer;
                }

                if (__gInstance.KillTarget != null)
                    __gInstance.KillTarget.myRend.material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void KillButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.KillTarget != null)
                {
                    if (__gInstance.KillTarget.IsOnAlert())
                    {
                        if (__gInstance.KillTarget.IsShielded())
                        {
                            var medic = __gInstance.KillTarget.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(__gInstance.KillTarget.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks) __gInstance.LastKill = DateTime.UtcNow;

                            StopKill.BreakShield(medic, __gInstance.KillTarget.PlayerId,
                                CustomGameOptions.ShieldBreaks);
                            Utils.RpcMurderPlayer(__gInstance.KillTarget, __gInstance.Player);
                        }
                        else if (__gInstance.Player.IsShielded())
                        {
                            var medic = __gInstance.Player.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(__gInstance.Player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks) __gInstance.LastKill = DateTime.UtcNow;

                            StopKill.BreakShield(medic, __gInstance.Player.PlayerId,
                                CustomGameOptions.ShieldBreaks);
                            if (CustomGameOptions.KilledOnAlert)
                            {
                                Utils.RpcMurderPlayer(__gInstance.Player, __gInstance.KillTarget);
                            }
                        }
                        else
                        {
                            Utils.RpcMurderPlayer(__gInstance.KillTarget, __gInstance.Player);
                            if (CustomGameOptions.KilledOnAlert)
                            {
                                Utils.RpcMurderPlayer(__gInstance.Player, __gInstance.KillTarget);
                            }
                        }

                        return;
                    }
                    else if (__gInstance.KillTarget.IsShielded())
                    {
                        var medic = __gInstance.KillTarget.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(__gInstance.KillTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        if (CustomGameOptions.ShieldBreaks) __gInstance.LastKill = DateTime.UtcNow;

                        StopKill.BreakShield(medic, __gInstance.KillTarget.PlayerId,
                            CustomGameOptions.ShieldBreaks);

                        return;
                    }

                    __gInstance.LastKill = DateTime.UtcNow;
                    __gInstance.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown);
                    Utils.RpcMurderPlayer(__gInstance.Player, __gInstance.KillTarget);
                }
            }
        }

        public static class HackButtonHandler
        {
            public static void HackButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.HackButton == null)
                {
                    __gInstance.HackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.HackButton.gameObject.SetActive(true);
                    __gInstance.HackButton.graphic.enabled = true;
                }

                __gInstance.HackButton.graphic.sprite = HackSprite;

                __gInstance.HackButton.gameObject.SetActive(__instance.UseButton.isActiveAndEnabled && !MeetingHud.Instance &&
                                                            !__gInstance.Player.Data.IsDead);
                __gInstance.HackButton.transform.position = new Vector3(__gInstance.MimicButton.transform.position.x,
                    __gInstance.HackButton.transform.position.y, __instance.ReportButton.transform.position.z);
                __gInstance.HackButton.SetCoolDown(
                    CustomGameOptions.HackCooldown - (float)(DateTime.UtcNow - __gInstance.LastHack).TotalSeconds,
                    CustomGameOptions.HackCooldown);

                __gInstance.HackButton.SetTarget(null);
                __gInstance.HackTarget = null;

                if (__gInstance.HackButton.isActiveAndEnabled)
                {
                    PlayerControl closestPlayer = null;
                    Utils.SetTarget(
                        ref closestPlayer,
                        __gInstance.HackButton,
                        GameOptionsData.KillDistances[CustomGameOptions.GlitchHackDistance]
                    );
                    __gInstance.HackTarget = closestPlayer; 
                }

                if (__gInstance.HackTarget != null)
                    __gInstance.HackTarget.myRend.material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void HackButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.HackTarget != null)
                {
                    if (__gInstance.HackTarget.IsOnAlert())
                    {
                        if (__gInstance.Player.IsShielded())
                        {
                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer2.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);

                            System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                            if (CustomGameOptions.ShieldBreaks)
                                __gInstance.LastHack = DateTime.UtcNow;
                            StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else
                        {
                            Utils.RpcMurderPlayer(__gInstance.HackTarget, __gInstance.Player);
                        }
                        return;
                    }

                    __gInstance.LastHack = DateTime.UtcNow;
                    //System.Console.WriteLine("Hacking " + __gInstance.HackTarget.Data.PlayerName + "...");
                    __gInstance.RpcSetHacked(__gInstance.HackTarget);
                }
            }
        }

        public static class MimicButtonHandler
        {
            public static void MimicButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.MimicButton == null)
                {
                    __gInstance.MimicButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.MimicButton.gameObject.SetActive(true);
                    __gInstance.MimicButton.graphic.enabled = true;
                }

                __gInstance.MimicButton.graphic.sprite = MimicSprite;

                __gInstance.MimicButton.gameObject.SetActive(__instance.UseButton.isActiveAndEnabled && !MeetingHud.Instance &&
                                                             !__gInstance.Player.Data.IsDead);
                __gInstance.MimicButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                    __instance.UseButton.transform.position.y, __instance.UseButton.transform.position.z);

                if (!__gInstance.MimicButton.isCoolingDown && !__gInstance.IsUsingMimic)
                {
                    __gInstance.MimicButton.isCoolingDown = false;
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.MimicButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.MimicButton.isCoolingDown = true;
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.MimicButton.graphic.color = Palette.DisabledClear;
                }

                if (!__gInstance.IsUsingMimic)
                    __gInstance.MimicButton.SetCoolDown(
                        CustomGameOptions.MimicCooldown -
                        (float)(DateTime.UtcNow - __gInstance.LastMimic).TotalSeconds,
                        CustomGameOptions.MimicCooldown);
            }

            public static void MimicButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.MimicList == null)
                {
                    HudManager.Instance.Chat.SetVisible(false);
                    __gInstance.MimicList = Object.Instantiate(HudManager.Instance.Chat);

                    __gInstance.MimicList.transform.SetParent(Camera.main.transform);
                    __gInstance.MimicList.SetVisible(true);
                    __gInstance.MimicList.Toggle();

                    __gInstance.MimicList.TextBubble.enabled = false;
                    __gInstance.MimicList.TextBubble.gameObject.SetActive(false);

                    __gInstance.MimicList.TextArea.enabled = false;
                    __gInstance.MimicList.TextArea.gameObject.SetActive(false);

                    __gInstance.MimicList.BanButton.enabled = false;
                    __gInstance.MimicList.BanButton.gameObject.SetActive(false);

                    __gInstance.MimicList.CharCount.enabled = false;
                    __gInstance.MimicList.CharCount.gameObject.SetActive(false);

                    __gInstance.MimicList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    __gInstance.MimicList.OpenKeyboardButton.Destroy();

                    __gInstance.MimicList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>()
                        .enabled = false;
                    __gInstance.MimicList.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                    __gInstance.MimicList.BackgroundImage.enabled = false;

                    foreach (var rend in __gInstance.MimicList.Content
                        .GetComponentsInChildren<SpriteRenderer>())
                        if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                        {
                            rend.enabled = false;
                            rend.gameObject.SetActive(false);
                        }

                    foreach (var bubble in __gInstance.MimicList.chatBubPool.activeChildren)
                    {
                        bubble.enabled = false;
                        bubble.gameObject.SetActive(false);
                    }

                    __gInstance.MimicList.chatBubPool.activeChildren.Clear();

                    foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x =>
                        x != null &&
                        x.Data != null &&
                        x != PlayerControl.LocalPlayer &&
                        !x.Data.Disconnected))
                    {
                        if (!player.Data.IsDead)
                            __gInstance.MimicList.AddChat(player, "Click here");
                        else
                        {
                            var deadBodies = Object.FindObjectsOfType<DeadBody>();
                            foreach (var body in deadBodies)
                                if (body.ParentId == player.PlayerId)
                                {
                                    player.Data.IsDead = false;
                                    __gInstance.MimicList.AddChat(player, "Click here");
                                    player.Data.IsDead = true;
                                }
                        }
                    }
                }
                else
                {
                    __gInstance.MimicList.Toggle();
                    __gInstance.MimicList.SetVisible(false);
                    __gInstance.MimicList = null;
                }
            }
        }
    }
}