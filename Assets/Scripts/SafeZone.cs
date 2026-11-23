using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Transform visual;
    public float startRadius = 180f;
    public float endRadius = 25f;
    public float shrinkDuration = 480f;
    public float damagePerSecondOutside = 5f;
    public float startDelay = 30f;

    public float CurrentRadius { get; private set; }
    private float elapsed;

    void Start()
    {
        CurrentRadius = startRadius;
        UpdateVisual();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01((elapsed - startDelay) / Mathf.Max(1f, shrinkDuration));
        CurrentRadius = Mathf.Lerp(startRadius, endRadius, t);
        UpdateVisual();

        // Local prototype only. In multiplayer, the authoritative server must apply zone damage.
        foreach (Health health in FindObjectsOfType<Health>())
        {
            Vector3 delta = health.transform.position - transform.position;
            delta.y = 0f;
            if (delta.magnitude > CurrentRadius)
                health.ApplyDamage(damagePerSecondOutside * Time.deltaTime);
        }
    }

    void UpdateVisual()
    {
        if (!visual) return;
        visual.position = new Vector3(transform.position.x, visual.position.y, transform.position.z);
        visual.localScale = new Vector3(CurrentRadius * 2f, visual.localScale.y, CurrentRadius * 2f);
    }
}
