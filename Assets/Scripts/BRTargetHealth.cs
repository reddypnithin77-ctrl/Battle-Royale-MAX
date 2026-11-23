using UnityEngine;

public class BRTargetHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    private void Awake() { CurrentHealth = maxHealth; }
    public void ApplyDamage(float amount)
    {
        if (amount <= 0f || CurrentHealth <= 0f) return;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        if (CurrentHealth <= 0f) Destroy(gameObject);
    }
    public void Restore(float amount) { CurrentHealth = Mathf.Clamp(CurrentHealth + Mathf.Max(0f, amount), 0f, maxHealth); }
}
