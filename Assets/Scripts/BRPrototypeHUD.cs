using UnityEngine;
using UnityEngine.UI;

// Optional simple HUD using legacy Unity UI Text fields.
public class BRPrototypeHUD : MonoBehaviour
{
    public BRHitscanWeapon weapon;
    public Health playerHealth;
    public Text ammoText;
    public Text healthText;
    public Text zoneText;

    private void Update()
    {
        if (weapon != null && ammoText != null)
            ammoText.text = weapon.IsReloading ? "RELOADING..." : weapon.AmmoInMagazine + " / " + weapon.ReserveAmmo;
        if (playerHealth != null && healthText != null)
            healthText.text = "HP " + Mathf.CeilToInt(playerHealth.Current);
        SafeZone zone = FindObjectOfType<SafeZone>();
        if (zone != null && zoneText != null)
            zoneText.text = "SAFE ZONE " + Mathf.CeilToInt(zone.CurrentRadius) + "m";
    }
}
