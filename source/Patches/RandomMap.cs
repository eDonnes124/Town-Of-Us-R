using HarmonyLib;
using System;
using TownOfUs.Patches;
using TownOfUs.CustomOption;
using AmongUs.GameOptions;

namespace TownOfUs
{
    [HarmonyPatch]
    class RandomMap
    {
        public static byte previousMap;
        public static float vision;
        public static int commonTasks;
        public static int shortTasks;
        public static int longTasks;

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        [HarmonyPrefix]
        public static bool Prefix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                previousMap = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                vision = GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                commonTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
                shortTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;
                longTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
                byte map = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                if (CustomGameOptions.RandomMapEnabled)
                {
                    map = GetRandomMap();
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId = map;
                }
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                Utils.Rpc(CustomRPC.SetSettings, map);
                if (CustomGameOptions.AutoAdjustSettings) AdjustSettings(map);
            }
            return true;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        [HarmonyPostfix]
        public static void Postfix(AmongUsClient __instance)
        {
            if (__instance.AmHost)
            {
                if (CustomGameOptions.AutoAdjustSettings)
                {
                    if (CustomGameOptions.SmallMapHalfVision && vision != 0) GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = vision;
                    if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1) AdjustCooldowns(CustomGameOptions.SmallMapDecreasedCooldown);
                    if (GameOptionsManager.Instance.currentNormalGameOptions.MapId >= 4) AdjustCooldowns(-CustomGameOptions.LargeMapIncreasedCooldown);
                }
                if (CustomGameOptions.RandomMapEnabled) GameOptionsManager.Instance.currentNormalGameOptions.MapId = previousMap;
                if (!(commonTasks == 0 && shortTasks == 0 && longTasks == 0))
                {
                    GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = commonTasks;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = shortTasks;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = longTasks;
                }
            }
        }

        public static byte GetRandomMap()
        {
            Random _rnd = new Random();
            float totalWeight = 0;
            totalWeight += CustomGameOptions.RandomMapSkeld;
            totalWeight += CustomGameOptions.RandomMapMira;
            totalWeight += CustomGameOptions.RandomMapPolus;
            totalWeight += CustomGameOptions.RandomMapAirship;
            totalWeight += CustomGameOptions.RandomMapFungle;
            if (SubmergedCompatibility.Loaded) totalWeight += CustomGameOptions.RandomMapSubmerged;

            if (totalWeight == 0) return GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            float randomNumber = _rnd.Next(0, (int)totalWeight);
            if (randomNumber < CustomGameOptions.RandomMapSkeld) return 0;
            randomNumber -= CustomGameOptions.RandomMapSkeld;
            if (randomNumber < CustomGameOptions.RandomMapMira) return 1;
            randomNumber -= CustomGameOptions.RandomMapMira;
            if (randomNumber < CustomGameOptions.RandomMapPolus) return 2;
            randomNumber -= CustomGameOptions.RandomMapPolus;
            if (randomNumber < CustomGameOptions.RandomMapAirship) return 4;
            randomNumber -= CustomGameOptions.RandomMapAirship;
            if (randomNumber < CustomGameOptions.RandomMapFungle) return 5;
            randomNumber -= CustomGameOptions.RandomMapFungle;
            if (SubmergedCompatibility.Loaded && randomNumber < CustomGameOptions.RandomMapSubmerged) return 6;

            return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        }

        public static void AdjustSettings(byte map)
        {
            if (map <= 1)
            {
                if (CustomGameOptions.SmallMapHalfVision) GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod *= 0.5f;
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
            }
            if (map == 1) AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);
            if (map >= 4)
            {
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
                AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
            }
            return;
        }

        public static void AdjustCooldowns(float change)
        {
            Generate.ExamineCooldown.Set((float)Generate.ExamineCooldown.Value + change, false);
            Generate.SeerCooldown.Set((float)Generate.SeerCooldown.Value + change, false);
            Generate.TrackCooldown.Set((float)Generate.TrackCooldown.Value + change, false);
            Generate.TrapCooldown.Set((float)Generate.TrapCooldown.Value + change, false);
            Generate.SheriffKillCd.Set((float)Generate.SheriffKillCd.Value + change, false);
            Generate.AlertCooldown.Set((float)Generate.AlertCooldown.Value + change, false);
            Generate.TransportCooldown.Set((float)Generate.TransportCooldown.Value + change, false);
            Generate.ProtectCd.Set((float)Generate.ProtectCd.Value + change, false);
            Generate.VestCd.Set((float)Generate.VestCd.Value + change, false);
            Generate.DouseCooldown.Set((float)Generate.DouseCooldown.Value + change, false);
            Generate.InfectCooldown.Set((float)Generate.InfectCooldown.Value + change, false);
            Generate.PestKillCooldown.Set((float)Generate.PestKillCooldown.Value + change, false);
            Generate.MimicCooldownOption.Set((float)Generate.MimicCooldownOption.Value + change, false);
            Generate.HackCooldownOption.Set((float)Generate.HackCooldownOption.Value + change, false);
            Generate.GlitchKillCooldownOption.Set((float)Generate.GlitchKillCooldownOption.Value + change, false);
            Generate.RampageCooldown.Set((float)Generate.RampageCooldown.Value + change, false);
            Generate.GrenadeCooldown.Set((float)Generate.GrenadeCooldown.Value + change, false);
            Generate.MorphlingCooldown.Set((float)Generate.MorphlingCooldown.Value + change, false);
            Generate.SwoopCooldown.Set((float)Generate.SwoopCooldown.Value + change, false);
            Generate.MineCooldown.Set((float)Generate.MineCooldown.Value + change, false);
            Generate.DragCooldown.Set((float)Generate.DragCooldown.Value + change, false);
            Generate.EscapeCooldown.Set((float)Generate.EscapeCooldown.Value + change, false);
            Generate.JuggKillCooldown.Set((float)Generate.JuggKillCooldown.Value + change, false);
            Generate.ObserveCooldown.Set((float)Generate.ObserveCooldown.Value + change, false);
            Generate.BiteCooldown.Set((float)Generate.BiteCooldown.Value + change, false);
            Generate.StakeCooldown.Set((float)Generate.StakeCooldown.Value + change, false);
            Generate.ConfessCooldown.Set((float)Generate.ConfessCooldown.Value + change, false);
            Generate.ChargeUpDuration.Set((float)Generate.ChargeUpDuration.Value + change, false);
            Generate.AbilityCooldown.Set((float)Generate.AbilityCooldown.Value + change, false);
            Generate.RadiateCooldown.Set((float)Generate.RadiateCooldown.Value + change, false);
            Generate.ReviveCooldown.Set((float)Generate.ReviveCooldown.Value + change, false);
            Generate.WhisperCooldown.Set((float)Generate.WhisperCooldown.Value + change, false);
            GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown += change;
            if (change % 5 != 0)
            {
                if (change > 0) change -= 2.5f;
                else if (change < 0) change += 2.5f;
            }
            GameOptionsManager.Instance.currentNormalGameOptions.EmergencyCooldown += (int)change;
            return;
        }
    }
}