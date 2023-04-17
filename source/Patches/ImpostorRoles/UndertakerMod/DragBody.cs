using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class DragBody
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Undertaker)) return;
            var role = Role.GetRole<Undertaker>(__instance);
            var body = role.CurrentlyDragging;
            if (body == null) return;
            if (__instance.Data.IsDead)
            {
                role.CurrentlyDragging = null;
                foreach (var body2 in body.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                return;
            }
            var currentPosition = __instance.transform.position;
            var velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
            Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 3) + body.myCollider.offset;
            newPos.z = currentPosition.z;

            //WHY ARE THERE DIFFERENT LOCAL Z INDEXS FOR DIFFERENT DECALS ON DIFFERENT LEVELS?!?!?!
            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (newPos.y > -7f)
                {
                    newPos.z = 0.0208f;
                } else
                {
                    newPos.z = -0.0273f;
                }
            }

            if (!PhysicsHelpers.AnythingBetween(
                currentPosition,
                newPos,
                Constants.ShipAndObjectsMask,
                false
            )) body.transform.position = newPos;
            if (!__instance.AmOwner) return;
            foreach (var body2 in body.bodyRenderers)
            {
                body2.material.SetColor("_OutlineColor", Color.green);
                body2.material.SetFloat("_Outline", 1f);
            }
        }
    }
}
