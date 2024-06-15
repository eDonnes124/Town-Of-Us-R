using System.Collections;
using System.Linq;
using TownOfUs.CrewmateRoles.DetectiveMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    public class Coroutine
    {
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");

        public static IEnumerator CleanCoroutine(DeadBody body, Janitor role)
        {
            KillButtonTarget.SetTarget(DestroyableSingleton<HudManager>.Instance.KillButton, null, role);
            role.Player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            var scene = Object.FindObjectsOfType<CrimeScene>().Where(cs => cs.DeadPlayer.PlayerId == body.ParentId).FirstOrDefault();
            SpriteRenderer renderer = null;
            SpriteRenderer crimeSceneRenderer = null;
            if (scene != null) crimeSceneRenderer = scene.gameObject.GetComponent<SpriteRenderer>();
            foreach (var body2 in body.bodyRenderers) renderer = body2;
            var backColor = renderer.material.GetColor(BackColor);
            var bodyColor = renderer.material.GetColor(BodyColor);
            var newColor = new Color(1f, 1f, 1f, 0f);
            for (var i = 0; i < 60; i++)
            {
                if (body == null) yield break;
                renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                if (crimeSceneRenderer != null)
                {
                    crimeSceneRenderer.color = Color.Lerp(crimeSceneRenderer.color, newColor, i / 60f);
                }
                yield return null;
            }

            if(scene != null)
            {
                foreach (var detRole in Role.GetRoles(RoleEnum.Detective))
                {
                    var detective = (Detective)detRole;
                    if (detective.CrimeScenes.Contains(scene.gameObject))
                        detective.CrimeScenes.Remove(scene.gameObject);
                    if (detective.InvestigatingScene == scene)
                    {
                        detective.InvestigatingScene = null;
                        detective.InvestigatedPlayers.Clear();
                    }
                }
                Object.Destroy(scene.gameObject);
            }
            Object.Destroy(body.gameObject);
        }
    }
}