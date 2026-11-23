using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
    [Min(1f)] public float healAmount = 35f;
    public bool destroyAfterUse = true;
    public string playerTag = "Player";

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;
        float before = health.Current;
        health.Restore(healAmount);
        if (health.Current > before && destroyAfterUse) Destroy(gameObject);
    }
}
