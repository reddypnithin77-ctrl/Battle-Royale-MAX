using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BRWeaponPickup : MonoBehaviour
{
    [Tooltip("Index in the player's BRWeaponLoadout weaponSlots array.")]
    public int weaponSlot;
    public string playerTag = "Player";
    public bool collectOnTouch = true;

    private void Reset() { GetComponent<Collider>().isTrigger = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (collectOnTouch) TryCollect(other);
    }

    public void TryCollect(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        BRWeaponLoadout loadout = other.GetComponentInParent<BRWeaponLoadout>();
        if (loadout == null) return;
        loadout.Equip(weaponSlot);
        Destroy(gameObject);
    }
}
