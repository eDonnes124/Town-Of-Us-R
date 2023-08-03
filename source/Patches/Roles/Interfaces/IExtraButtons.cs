using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public interface IExtraButton
    {
        KillButton RoleAbilityButton { get; set; }

        virtual void SetupAndActive(IExtraButton role)
        {
            if (role.RoleAbilityButton == null)
            {
                role.RoleAbilityButton = Object.Instantiate(HudManager.Instance?.KillButton, HudManager.Instance?.KillButton.transform.parent);
                role.RoleAbilityButton.graphic.enabled = true;
                role.RoleAbilityButton.gameObject.SetActive(false);
            }

            if (((Role)role).Faction == Faction.NeutralKilling) role.RoleAbilityButton.transform.localPosition = new(-2f, 0f, 0f);

            role.RoleAbilityButton.gameObject.SetActive((HudManager.Instance?.UseButton.isActiveAndEnabled == true || 
                                                         HudManager.Instance?.PetButton.isActiveAndEnabled == true)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);
        }
    }

    public interface IExtraButtons
    {
        List<KillButton> ExtraButtons { get; set; }
    }
}