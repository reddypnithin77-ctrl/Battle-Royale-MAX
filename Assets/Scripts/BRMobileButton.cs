using UnityEngine;
using UnityEngine.EventSystems;

// Add this component to UI buttons. Set Action in the Inspector.
public class BRMobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public enum ButtonAction { Fire, Jump, Reload, NextWeapon, PreviousWeapon }
    public ButtonAction action;
    public BRWeaponLoadout loadout;
    private bool fireButton;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (action != ButtonAction.Fire) return;
        fireButton = true;
        if (MobileInputBridge.Instance != null) MobileInputBridge.Instance.SetFire(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!fireButton) return;
        fireButton = false;
        if (MobileInputBridge.Instance != null) MobileInputBridge.Instance.SetFire(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (action)
        {
            case ButtonAction.Jump:
                if (MobileInputBridge.Instance != null) MobileInputBridge.Instance.PressJump();
                break;
            case ButtonAction.Reload:
                BRHitscanWeapon weapon = GetActiveWeapon();
                if (weapon != null) weapon.Reload();
                break;
            case ButtonAction.NextWeapon:
                if (loadout != null) loadout.NextWeapon();
                break;
            case ButtonAction.PreviousWeapon:
                if (loadout != null) loadout.PreviousWeapon();
                break;
        }
    }

    private BRHitscanWeapon GetActiveWeapon()
    {
        if (loadout != null && loadout.weaponSlots != null && loadout.weaponSlots.Length > loadout.ActiveSlot)
            return loadout.weaponSlots[loadout.ActiveSlot].GetComponentInChildren<BRHitscanWeapon>();
        return GetComponentInParent<BRHitscanWeapon>();
    }

    private void OnDisable()
    {
        if (fireButton && MobileInputBridge.Instance != null) MobileInputBridge.Instance.SetFire(false);
        fireButton = false;
    }
}
