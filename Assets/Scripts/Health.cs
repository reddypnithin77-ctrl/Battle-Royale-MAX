using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Min(1)] public float maxHealth = 100f;
    public float Current { get; private set; }
    public event Action<float, float> Changed;
    public event Action Died;

    void Awake()
    {
        maxHealth = IsFinite(maxHealth) ? Mathf.Max(1f, maxHealth) : 100f;
        Current = maxHealth;
    }

    public void ApplyDamage(float amount)
    {
        if (!IsFinite(amount) || amount <= 0f || Current <= 0f) return;
        if (!IsFinite(Current)) Current = maxHealth;
        Current = Mathf.Max(0f, Current - amount);
        Changed?.Invoke(Current, maxHealth);
        if (Current <= 0f) Died?.Invoke();
    }

    public void Restore(float amount)
    {
        if (!IsFinite(amount) || amount <= 0f || Current <= 0f) return;
        if (!IsFinite(Current)) Current = maxHealth;
        Current = Mathf.Min(maxHealth, Current + amount);
        Changed?.Invoke(Current, maxHealth);
    }
    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
}
