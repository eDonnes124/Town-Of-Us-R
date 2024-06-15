using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    public class CrimeScene : MonoBehaviour
    {
        public List<byte> ScenePlayers = new List<byte>();
        public PlayerControl DeadPlayer = null;
        public CrimeScene(IntPtr ptr) : base(ptr) { }

        void FixedUpdate()
        {
            foreach(var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead) continue;
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                Debug.Log(GetComponent<BoxCollider2D>().IsTouching(player.Collider));
                if (Vector2.Distance(player.GetTruePosition(), gameObject.transform.position) >
                      GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance]) continue;
                //if (!GetComponent<BoxCollider2D>().IsTouching(player.Collider)) continue;
                if (!ScenePlayers.Contains(player.PlayerId) && player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                {
                    Debug.Log(player.name + " contaminated the crime scene");
                    ScenePlayers.Add(player.PlayerId);
                }
            }
        }
    }

    [HarmonyPatch]
    public static class CrimeSceneExtensions
    {
        public static void ClearCrimeScenes(this List<GameObject> obj)
        {
            foreach (GameObject t in obj)
            {
                UnityEngine.Object.Destroy(t);
            }
            obj.Clear();
        }

        public static GameObject CreateCrimeScene(this Vector3 location, PlayerControl victim)
        {
            GameObject bloodSplat = new GameObject("CrimeScene");
            bloodSplat.transform.position = location;
            bloodSplat.layer = LayerMask.NameToLayer("Players");
            SpriteRenderer render = bloodSplat.AddComponent<SpriteRenderer>();
            render.sprite = TownOfUs.CrimeSceneSprite;
            Vector3 scale = render.transform.localScale;
            scale.x *= 0.5f;
            scale.y *= 0.5f;
            render.transform.localScale = scale;
            BoxCollider2D splatCollider = bloodSplat.AddComponent<BoxCollider2D>();
            splatCollider.size = new Vector2(render.size.x, render.size.y);
            var scene = bloodSplat.AddComponent<CrimeScene>();
            scene.DeadPlayer = victim;
            return bloodSplat;
        }
    }
}