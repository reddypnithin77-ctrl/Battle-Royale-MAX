using System.Collections;
using UnityEngine;

public class BRHitscanWeapon : MonoBehaviour
{
    [Header("Weapon")]
    public Camera aimCamera;
    public float damage = 20f;
    public float range = 180f;
    public float shotsPerSecond = 7f;
    [Header("Ammo")]
    public int magazineSize = 30;
    public int reserveAmmo = 120;
    public float reloadSeconds = 1.6f;

    public int AmmoInMagazine { get; private set; }
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading { get; private set; }
    private float nextShot;

    private void Awake() { AmmoInMagazine = magazineSize; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
        bool firing = MobileInputBridge.Instance != null ? MobileInputBridge.Instance.FireHeld : Input.GetMouseButton(0);
        if (firing && !IsReloading && Time.time >= nextShot)
        {
            if (AmmoInMagazine <= 0) { Reload(); return; }
            nextShot = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);
            Fire();
            AmmoInMagazine--;
        }
    }

    public void Reload()
    {
        if (!IsReloading && AmmoInMagazine < magazineSize && reserveAmmo > 0)
            StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(reloadSeconds);
        int needed = magazineSize - AmmoInMagazine;
        int loaded = Mathf.Min(needed, reserveAmmo);
        AmmoInMagazine += loaded;
        reserveAmmo -= loaded;
        IsReloading = false;
    }

    public void AddAmmo(int amount)
    {
        if (amount > 0) reserveAmmo += amount;
    }

    private void Fire()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        if (aimCamera == null) return;
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            BRTargetHealth health = hit.collider.GetComponentInParent<BRTargetHealth>();
            if (health != null) health.ApplyDamage(damage);
        }
    }
}
