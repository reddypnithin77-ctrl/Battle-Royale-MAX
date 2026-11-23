using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Transform visual;
    public float startRadius = 180f;
    public float endRadius = 25f;
    public float shrinkDuration = 480f;
    public float damagePerSecondOutside = 5f;
    public float startDelay = 30f;
    public float healthRefreshSeconds = 1f;

    public float CurrentRadius { get; private set; }
    private float elapsed;
    private float refreshTimer;
    private readonly List<Health> trackedHealth = new List<Health>();

    private void Start()
    {
        CurrentRadius = startRadius;
        RefreshHealthList();
        UpdateVisual();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= healthRefreshSeconds)
        {
            refreshTimer = 0f;
            RefreshHealthList();
        }

        float t = Mathf.Clamp01((elapsed - startDelay) / Mathf.Max(1f, shrinkDuration));
        CurrentRadius = Mathf.Lerp(startRadius, endRadius, t);
        UpdateVisual();

        // Local prototype only. Multiplayer zone damage must be server-authoritative.
        for (int i = trackedHealth.Count - 1; i >= 0; i--)
        {
            Health health = trackedHealth[i];
            if (health == null) { trackedHealth.RemoveAt(i); continue; }
            Vector3 delta = health.transform.position - transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > CurrentRadius * CurrentRadius)
                health.ApplyDamage(damagePerSecondOutside * Time.deltaTime);
        }
    }

    private void RefreshHealthList()
    {
        trackedHealth.Clear();
        trackedHealth.AddRange(FindObjectsOfType<Health>());
    }

    private void UpdateVisual()
    {
        if (visual == null) return;
        visual.position = new Vector3(transform.position.x, visual.position.y, transform.position.z);
        visual.localScale = new Vector3(CurrentRadius * 2f, visual.localScale.y, CurrentRadius * 2f);
    }
}
