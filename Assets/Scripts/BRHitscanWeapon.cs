using System.Collections;
using UnityEngine;

public class BRHitscanWeapon : MonoBehaviour
{
    [Header("Weapon")]
    public Camera aimCamera;
    public float damage = 20f;
    public float range = 180f;
    public float shotsPerSecond = 7f;
    [Header("Ballistics")]
    [Tooltip("Maximum random angular spread in degrees.")]
    public float hipFireSpread = 1.1f;
    public float movingSpreadBonus = 0.8f;
    public float recoilPitchPerShot = 0.45f;
    public float recoilYawPerShot = 0.18f;
    public float recoilRecovery = 5f;
    [Header("Ammo")]
    public int magazineSize = 30;
    public int reserveAmmo = 120;
    public float reloadSeconds = 1.6f;

    public int AmmoInMagazine { get; private set; }
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading { get; private set; }
    public Vector2 CurrentRecoil { get; private set; }
    private float nextShot;
    private float recoilPitch;
    private float recoilYaw;

    private void Awake()
    {
        magazineSize = Mathf.Max(1, magazineSize);
        reserveAmmo = Mathf.Max(0, reserveAmmo);
        AmmoInMagazine = magazineSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
        // Keep mouse testing available in the Editor even when the mobile input bridge exists.
        bool mobileFire = MobileInputBridge.Instance != null && MobileInputBridge.Instance.FireHeld;
        bool firing = mobileFire || Input.GetMouseButton(0);
        if (firing && !IsReloading && Time.time >= nextShot)
        {
            if (AmmoInMagazine <= 0) { Reload(); return; }
            nextShot = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);
            Fire();
            AmmoInMagazine--;
        }

        recoilPitch = Mathf.MoveTowards(recoilPitch, 0f, recoilRecovery * Time.deltaTime);
        recoilYaw = Mathf.MoveTowards(recoilYaw, 0f, recoilRecovery * Time.deltaTime);
        CurrentRecoil = new Vector2(recoilPitch, recoilYaw);
    }

    public void Reload()
    {
        if (!IsReloading && AmmoInMagazine < magazineSize && reserveAmmo > 0)
            StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(Mathf.Max(0f, reloadSeconds));
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

        recoilPitch += recoilPitchPerShot;
        recoilYaw += Random.Range(-recoilYawPerShot, recoilYawPerShot);

        float spread = hipFireSpread;
        if (MobileInputBridge.Instance != null && MobileInputBridge.Instance.Move.sqrMagnitude > 0.05f)
            spread += movingSpreadBonus;

        Vector3 direction = aimCamera.transform.forward;
        direction = Quaternion.AngleAxis(Random.Range(-spread, spread), aimCamera.transform.up) * direction;
        direction = Quaternion.AngleAxis(Random.Range(-spread, spread), aimCamera.transform.right) * direction;

        Ray ray = new Ray(aimCamera.transform.position, direction.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            BRTargetHealth health = hit.collider.GetComponentInParent<BRTargetHealth>();
            if (health != null) health.ApplyDamage(damage);
        }
    }
}
