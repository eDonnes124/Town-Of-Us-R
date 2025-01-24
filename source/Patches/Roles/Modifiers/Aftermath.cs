using Reactor.Utilities;
using System;
using System.Collections;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.BomberMod;
using UnityEngine;
using TownOfUs.Modifiers.UnderdogMod;
using Object = UnityEngine.Object;
using Hazel;
using TownOfUs.Patches;
using System.Linq;

namespace TownOfUs.Roles.Modifiers
{
    public class Aftermath : Modifier
    {
        public Aftermath(PlayerControl player) : base(player)
        {
            Name = "Aftermath";
            TaskText = () => "Force your killer to use their ability";
            Color = Patches.Colors.Aftermath;
            ModifierType = ModifierEnum.Aftermath;
        }

        public static void ForceAbility(PlayerControl player, PlayerControl corpse)
        {
            if (!player.AmOwner) return;
            DeadBody db = null;
            var bodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in bodies)
            {
                try
                {
                    if (body?.ParentId == corpse.PlayerId) { db = body; break; }
                }
                catch
                {
                }
            }
            Coroutines.Start(delay(player, corpse, db));
        }

        private static IEnumerator delay(PlayerControl player, PlayerControl corpse, DeadBody db)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            var role = Role.GetRole(player);

            if (role is Blackmailer blackmailer)
            {
                blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                if (blackmailer.Blackmailed != null && blackmailer.Blackmailed.Data.IsImpostor())
                {
                    if (blackmailer.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        blackmailer.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        blackmailer.Blackmailed.nameText().color = Patches.Colors.Impostor;
                    else blackmailer.Blackmailed.nameText().color = Color.clear;
                }
                blackmailer.Blackmailed = player;

                Utils.Rpc(CustomRPC.Blackmail, player.PlayerId, player.PlayerId);
            }
            else if (role is Glitch glitch)
            {
                if (glitch.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph) glitch.RpcSetMimicked(corpse);
            }
            else if (role is Escapist escapist)
            {
                if (escapist.EscapePoint != new Vector3(0f, 0f, 0f))
                {
                    Utils.Rpc(CustomRPC.Escape, PlayerControl.LocalPlayer.PlayerId, escapist.EscapePoint);
                    escapist.LastEscape = DateTime.UtcNow;
                    Escapist.Escape(escapist.Player);
                }
            }
            else if (role is Grenadier grenadier)
            {
                if (!grenadier.Enabled)
                {
                    grenadier.TimeRemaining = CustomGameOptions.GrenadeDuration;
                    grenadier.StartFlash();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.FlashGrenade, SendOption.Reliable, -1);
                    writer.Write((byte)grenadier.Player.PlayerId);
                    writer.Write((byte)grenadier.flashedPlayers.Count);
                    foreach (var player2 in grenadier.flashedPlayers)
                    {
                        writer.Write(player2.PlayerId);
                    }
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (role is Hypnotist hypnotist)
            {
                hypnotist.HypnotisedPlayers.Add(hypnotist.Player.PlayerId);
                if (hypnotist.HysteriaActive) hypnotist.Hysteria();
            }
            else if (role is Janitor janitor)
            {
                Utils.Rpc(CustomRPC.JanitorClean, PlayerControl.LocalPlayer.PlayerId, db.ParentId);

                Coroutines.Start(ImpostorRoles.JanitorMod.Coroutine.CleanCoroutine(db, janitor));
            }
            else if (role is Miner miner)
            {
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, miner.VentSize, 0);
                hits = hits.ToArray().Where(c =>
                        (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                    .ToArray();
                if (hits.Count == 0 && !SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1)
                {
                    var position = PlayerControl.LocalPlayer.transform.position;
                    var id = ImpostorRoles.MinerMod.PlaceVent.GetAvailableId();
                    Utils.Rpc(CustomRPC.Mine, id, PlayerControl.LocalPlayer.PlayerId, position, position.z + 0.0004f);
                    ImpostorRoles.MinerMod.PlaceVent.SpawnVent(id, miner, position, position.z + 0.0004f);
                    miner.LastMined = DateTime.UtcNow;
                }
            }
            else if (role is Morphling morphling)
            {
                if (morphling.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                {
                    Utils.Rpc(CustomRPC.Morph, PlayerControl.LocalPlayer.PlayerId, corpse.PlayerId);
                    morphling.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    if (morphling.SampledPlayer == null) morphling._morphButton.graphic.sprite = TownOfUs.MorphSprite;
                    morphling.SampledPlayer = corpse;
                    morphling.MorphedPlayer = corpse;
                    Utils.Morph(morphling.Player, corpse);
                }
            }
            else if (role is Swooper swooper)
            {
                if (swooper.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                {
                    Utils.Rpc(CustomRPC.Swoop, PlayerControl.LocalPlayer.PlayerId);
                    swooper.TimeRemaining = CustomGameOptions.SwoopDuration;
                    swooper.Swoop();
                }
            }
            else if (role is Undertaker undertaker)
            {
                if (undertaker.CurrentlyDragging)
                {
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (Patches.SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    Utils.Rpc(CustomRPC.Drop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = undertaker.CurrentlyDragging;
                    undertaker.CurrentlyDragging = null;
                    body.transform.position = position;
                }

                Utils.Rpc(CustomRPC.Drag, PlayerControl.LocalPlayer.PlayerId, db.ParentId);
                undertaker.CurrentlyDragging = db;
                ImpostorRoles.UndertakerMod.KillButtonTarget.SetTarget(undertaker._dragDropButton, null, undertaker);
                undertaker._dragDropButton.graphic.sprite = TownOfUs.DropSprite;

            }
            else if (role is Venerer venerer)
            {
                if (!venerer.Enabled)
                {
                    Utils.Rpc(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId, venerer.Kills);
                    venerer.TimeRemaining = CustomGameOptions.AbilityDuration;
                    venerer.KillsAtStartAbility = venerer.Kills;
                    venerer.Ability();
                }
            }
            else if (role is Bomber bomber)
            {
                bomber.Detonated = false;
                var pos = PlayerControl.LocalPlayer.transform.position;
                pos.z += 0.001f;
                bomber.DetonatePoint = pos;
                bomber.PlantButton.graphic.sprite = TownOfUs.DetonateSprite;
                bomber.TimeRemaining = CustomGameOptions.DetonateDelay;
                bomber.PlantButton.SetCoolDown(bomber.TimeRemaining, CustomGameOptions.DetonateDelay);
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                    PlayerControl.LocalPlayer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                bomber.Bomb = BombExtentions.CreateBomb(pos);
                if (CustomGameOptions.AllImpsSeeBomb) Utils.Rpc(CustomRPC.Plant, pos.x, pos.y, pos.z);
            }
        }
    }
}