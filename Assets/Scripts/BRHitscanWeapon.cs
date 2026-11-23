using UnityEngine;

public class BRHitscanWeapon : MonoBehaviour
{
    public Camera aimCamera;
    public float damage = 20f;
    public float range = 180f;
    public float shotsPerSecond = 7f;
    private float nextShot;
    private void Update()
    {
        bool firing = MobileInputBridge.Instance != null ? MobileInputBridge.Instance.FireHeld : Input.GetMouseButton(0);
        if (firing && Time.time >= nextShot) { nextShot = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond); Fire(); }
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
