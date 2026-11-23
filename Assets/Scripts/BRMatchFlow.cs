using UnityEngine;
using UnityEngine.UI;

// Local practice-match flow only; not a networked battle-royale session.
public class BRMatchFlow : MonoBehaviour
{
    public Health playerHealth;
    public Text statusText;
    public float startCountdown = 3f;
    public float botScanInterval = 1f;

    public bool MatchStarted { get; private set; }
    public bool MatchEnded { get; private set; }
    private float countdown;
    private float scanTimer;

    private void Start()
    {
        countdown = Mathf.Max(0f, startCountdown);
        if (playerHealth != null) playerHealth.Died += OnPlayerDied;
        SetStatus(countdown > 0f ? "MATCH STARTS IN " + Mathf.CeilToInt(countdown) : "ELIMINATE ALL TRAINING BOTS");
    }

    private void Update()
    {
        if (MatchEnded) return;
        if (!MatchStarted)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                MatchStarted = true;
                SetStatus("ELIMINATE ALL TRAINING BOTS");
            }
            else SetStatus("MATCH STARTS IN " + Mathf.CeilToInt(countdown));
            return;
        }

        scanTimer += Time.deltaTime;
        if (scanTimer < botScanInterval) return;
        scanTimer = 0f;
        BRTargetHealth[] targets = FindObjectsOfType<BRTargetHealth>();
        if (targets.Length == 0)
        {
            MatchEnded = true;
            SetStatus("PRACTICE COMPLETE!");
        }
    }

    private void OnPlayerDied()
    {
        MatchEnded = true;
        SetStatus("YOU WERE ELIMINATED");
    }

    private void SetStatus(string value)
    {
        if (statusText != null) statusText.text = value;
    }

    private void OnDestroy()
    {
        if (playerHealth != null) playerHealth.Died -= OnPlayerDied;
    }
}
