using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30;
    public string playerTag = "Player";

    private void Reset() { GetComponent<Collider>().isTrigger = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        BRHitscanWeapon weapon = other.GetComponentInChildren<BRHitscanWeapon>();
        if (weapon == null) return;
        weapon.AddAmmo(ammoAmount);
        Destroy(gameObject);
    }
}
