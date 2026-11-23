using UnityEngine;

public class BRTargetHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    private void Awake()
    {
        maxHealth = IsFinite(maxHealth) ? Mathf.Max(1f, maxHealth) : 100f;
        CurrentHealth = maxHealth;
    }
    public void ApplyDamage(float amount)
    {
        if (!IsFinite(amount) || amount <= 0f || CurrentHealth <= 0f) return;
        if (!IsFinite(CurrentHealth)) CurrentHealth = maxHealth;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        if (CurrentHealth <= 0f) Destroy(gameObject);
    }
    public void Restore(float amount)
    {
        if (!IsFinite(amount) || amount <= 0f || CurrentHealth <= 0f) return;
        if (!IsFinite(CurrentHealth)) CurrentHealth = maxHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
    }

    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
}
