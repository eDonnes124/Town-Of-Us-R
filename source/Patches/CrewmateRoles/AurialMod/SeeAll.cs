using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using Color = UnityEngine.Color;

namespace TownOfUs.CrewmateRoles.AurialMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class SeeAll
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return;

            Aurial s = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove && !s.Loaded) return;

            var button = __instance.KillButton;
            button.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            button.SetCoolDown(s.RadiateTimer(), CustomGameOptions.RadiateCooldown);

            var renderer = button.graphic;
            if (!button.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (!s.Loaded) s.Loaded = true;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (PlayerControl.LocalPlayer == player) continue;

                if (s.NormalVision)
                {
                    continue;
                }

                if (s.SeeDelay() != 0f)
                {
                    if (player.Is(RoleEnum.Mayor))
                    {
                        var mayor = Role.GetRole<Mayor>(player);
                        if (!mayor.Revealed)
                        {
                            ColorChar(player, Color.clear);
                            continue;
                        }
                    }
                    else
                    {
                        ColorChar(player, Color.clear);
                        continue;
                    }
                }

                if (!Check(s, player))
                {
                    ColorChar(player, Color.white);
                    continue;
                }

                var faction = Role.GetRole(player).Faction;
                switch (faction)
                {
                    default:
                        ColorChar(player, Color.white);
                        break;
                    case Faction.Crewmates:
                        ColorChar(player, Color.green);
                        break;
                    case Faction.Impostors:
                        ColorChar(player, Color.red);
                        break;
                    case Faction.NeutralBenign:
                    case Faction.NeutralEvil:
                    case Faction.NeutralKilling:
                        ColorChar(player, Color.gray);
                        break;
                }
            }
        }

        public static bool Check(Aurial s, PlayerControl p)
        {
            if (p == null) return false;
            if (s.knownPlayerRoles.TryGetValue(p.PlayerId, out var count))
            {
                if (count >= CustomGameOptions.RadiateCount) return true;
            }
            return false;
        }

        public static void ColorChar(PlayerControl p, Color c)
        {
            var fit = p.GetCustomOutfitType();
            if ((fit != CustomPlayerOutfitType.Aurial && fit != CustomPlayerOutfitType.Camouflage && fit != CustomPlayerOutfitType.Swooper) || (fit == CustomPlayerOutfitType.Aurial && p.myRend().color != c))
            {
                p.SetOutfit(CustomPlayerOutfitType.Aurial, new GameData.PlayerOutfit()
                {
                    ColorId = p.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    NamePlateId = p.GetDefaultOutfit().NamePlateId,
                    PlayerName = " ",
                    PetId = ""
                }); ;
                if (c == Color.red) p.cosmetics.SetBodyColor(0);
                if (c == Color.green) p.cosmetics.SetBodyColor(2);
                if (c == Color.white) p.cosmetics.SetBodyColor(7);
                if (c == Color.gray) p.cosmetics.SetBodyColor(15);
                p.myRend().color = c;
                p.nameText().color = Color.clear;
                if (c == Color.clear) p.cosmetics.colorBlindText.color = c;
                else p.cosmetics.colorBlindText.color = Color.white;
            }
        }

        public static void AllToNormal()
        {
            foreach (var p in PlayerControl.AllPlayerControls)
            {
                Utils.Unmorph(p);
                p.myRend().color = Color.white;
            }
        }
    }
}
