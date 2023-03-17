﻿using InnerNet;
using System.Linq;
using UnityEngine;

namespace TownOfUs.LocalGame
{
    public static class Utils
    {
        public static void CleanUpLoad()
        {
            if (GameData.Instance.AllPlayers.Count == 1)
            {
                InstanceControl.clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();
            }
        }
        public static PlayerControl CreatePlayerInstance(string name = "", int id = -1)
        {
            PlatformSpecificData samplePSD = new()
            {
                Platform = Platforms.StandaloneWin10,
                PlatformName = "Robot"
            };

            int sampleId = id;
            if (sampleId == -1) sampleId = InstanceControl.AvailableId();

            var sampleC = new ClientData(sampleId, name + $"-{sampleId}", samplePSD, 5, "", "");

            AmongUsClient.Instance.CreatePlayer(sampleC);
            AmongUsClient.Instance.allClients.Add(sampleC);

            sampleC.Character.SetName(name + $" {{{sampleC.Character.PlayerId}:{sampleId}}}");
            sampleC.Character.SetSkin(HatManager.Instance.allSkins[Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetColor(Random.Range(0, Palette.PlayerColors.Length));
            sampleC.Character.SetHat(HatManager.Instance.allHats[Random.Range(0, HatManager.Instance.allHats.Count)].ProdId, 0);

            InstanceControl.clients.Add(sampleId, sampleC);
            InstanceControl.PlayerIdClientId.Add(sampleC.Character.PlayerId, sampleId);
            sampleC.Character.MyPhysics.ResetAnimState();
            sampleC.Character.MyPhysics.ResetMoveState();
            return sampleC.Character;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                    return player;
            }
            return null;
        }

        public static void RemovePlayer(byte id)
        {
            int clientId = InstanceControl.clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
            InstanceControl.clients.Remove(clientId, out ClientData outputData);
            InstanceControl.PlayerIdClientId.Remove(id);
            AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.ExitGame);
            AmongUsClient.Instance.allClients.Remove(outputData);
        }

        public static void RemoveAllPlayers()
        {
            foreach (byte playerId in InstanceControl.PlayerIdClientId.Keys) RemovePlayer(playerId);
            InstanceControl.SwitchTo(AmongUsClient.Instance.allClients[0].Character.PlayerId);
        }
    }
}