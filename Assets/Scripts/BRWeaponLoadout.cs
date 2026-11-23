using UnityEngine;

public class BRWeaponLoadout : MonoBehaviour
{
    [Tooltip("Weapon GameObjects parented under the player, in selection order.")]
    public GameObject[] weaponSlots;
    public int startingSlot = 0;
    public int ActiveSlot { get; private set; }

    private void Start()
    {
        if (weaponSlots == null || weaponSlots.Length == 0) return;
        Equip(Mathf.Clamp(startingSlot, 0, weaponSlots.Length - 1));
    }

    public void Equip(int slot)
    {
        if (weaponSlots == null || slot < 0 || slot >= weaponSlots.Length || weaponSlots[slot] == null) return;
        ActiveSlot = slot;
        for (int i = 0; i < weaponSlots.Length; i++)
            if (weaponSlots[i] != null) weaponSlots[i].SetActive(i == ActiveSlot);
    }

    public void NextWeapon()
    {
        if (weaponSlots == null || weaponSlots.Length == 0) return;
        for (int step = 1; step <= weaponSlots.Length; step++)
        {
            int candidate = (ActiveSlot + step) % weaponSlots.Length;
            if (weaponSlots[candidate] != null) { Equip(candidate); return; }
        }
    }

    public void PreviousWeapon()
    {
        if (weaponSlots == null || weaponSlots.Length == 0) return;
        for (int step = 1; step <= weaponSlots.Length; step++)
        {
            int candidate = (ActiveSlot - step + weaponSlots.Length) % weaponSlots.Length;
            if (weaponSlots[candidate] != null) { Equip(candidate); return; }
        }
    }
}
