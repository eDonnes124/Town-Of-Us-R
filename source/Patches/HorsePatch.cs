using TownOfUs.Extensions;
using HarmonyLib;
using Reactor;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    class HorsePatch
    {




        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix()
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                if (CustomGameOptions.Horse) {
                    player.CurrentBodySprite = player.BodySprites[1];

                    //Animations
                    player.MyPhysics.CurrentAnimationGroup = player.MyPhysics.AnimationGroups[1];
                    

                    // Cosmetics to a degree - Its not animated but It displays and thats what matters.
                    GameObject HatOwner = new GameObject("hatown");
                    if (player.transform.Find("hatown") == null)
                    {
                        HatOwner = GameObject.Instantiate(HatOwner, player.NormalBodySprite.BodySprite.transform.position, Quaternion.identity);
                        HatOwner.name = "hatown";
                        HatOwner.transform.SetParent(player.CurrentBodySprite.BodySprite.transform);
                        player.HatRenderer.transform.SetParent(HatOwner.transform);
                        player.VisorSlot.transform.SetParent(HatOwner.transform);
                        //Loading horse outfits
                        var outfit = player.CurrentOutfit;
                        player.SetColor(outfit.ColorId);
                        player.SetSkin("", outfit.ColorId);
                        //player.SetHat("", outfit.ColorId);
                        //player.SetVisor("");
                        player.NormalBodySprite.Visible = false;


                    }
                    else
                    {
                        HatOwner = player.transform.Find("hatown").gameObject;
                    }


                    if (player.CurrentBodySprite.BodySprite.flipX)
                    {
                        HatOwner.transform.localPosition = new Vector3(-1.3f, -2.1f, 0f);
                        player.NormalBodySprite.BodySprite.flipX = true;

                    }
                    else
                    {
                        HatOwner.transform.localPosition = new Vector3(1.3f, -2.1f, 0f);
                        player.NormalBodySprite.BodySprite.flipX = false;

                    }





                    

                    

                } else
                {
                    if (player.transform.Find("hatown") != null)
                    {
                        player.CurrentBodySprite = player.BodySprites[0];
                        player.NormalBodySprite.Visible = true;
                        player.MyPhysics.CurrentAnimationGroup = player.MyPhysics.AnimationGroups[0];
                        var outfit = player.GetDefaultOutfit();
                        player.SetColor(outfit.ColorId);
                        player.SetSkin(outfit.SkinId, outfit.ColorId);
                        player.SetHat(outfit.HatId, outfit.ColorId);
                        player.SetVisor(outfit.VisorId);
                        player.HatRenderer.transform.SetParent(player.NormalBodySprite.BodySprite.transform);
                        player.VisorSlot.transform.SetParent(player.NormalBodySprite.BodySprite.transform);
                        Object.Destroy(player.transform.Find("hatown").gameObject);
                    }
                }
            }
        }




    }
}
