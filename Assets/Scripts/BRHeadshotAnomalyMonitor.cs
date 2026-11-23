using System.Collections.Generic;

// Tracks server-observed hit patterns. A suspicious ratio is a review signal,
// not proof of cheating and never an automatic ban.
public class BRHeadshotAnomalyMonitor
{
    public struct Result
    {
        public bool accepted;
        public bool suspicious;
        public string reason;
    }

    private class HitRecord
    {
        public float time;
        public bool headshot;
    }

    private class PlayerWindow
    {
        public readonly Queue<HitRecord> hits = new Queue<HitRecord>();
        public int consecutiveHeadshots;
        public float lastHitTime = -1f;
    }

    private readonly Dictionary<string, PlayerWindow> windows =
        new Dictionary<string, PlayerWindow>();

    private readonly int minimumHits;
    private readonly int suspiciousStreak;
    private readonly float suspiciousRatio;
    private readonly float windowSeconds;

    public BRHeadshotAnomalyMonitor(int minimumHits = 12, int suspiciousStreak = 8,
        float suspiciousRatio = 0.85f, float windowSeconds = 120f)
    {
        this.minimumHits = minimumHits < 1 ? 12 : minimumHits;
        this.suspiciousStreak = suspiciousStreak < 1 ? 8 : suspiciousStreak;
        this.suspiciousRatio = suspiciousRatio <= 0f || suspiciousRatio > 1f ? 0.85f : suspiciousRatio;
        this.windowSeconds = windowSeconds <= 0f ? 120f : windowSeconds;
    }

    public Result Evaluate(string attackerId, float serverTime, bool isHeadshot,
        bool serverLineOfSightClear)
    {
        if (string.IsNullOrWhiteSpace(attackerId) || !IsFinite(serverTime))
            return Reject("Invalid hit identity or server timestamp.", true);

        if (!serverLineOfSightClear)
            return Reject("Server line-of-sight blocked: possible wall hit.", true);

        PlayerWindow state;
        if (!windows.TryGetValue(attackerId, out state))
        {
            state = new PlayerWindow();
            windows.Add(attackerId, state);
        }

        if (state.lastHitTime >= 0f && serverTime < state.lastHitTime)
            return Reject("Hit timestamp moved backwards.", true);

        while (state.hits.Count > 0 && serverTime - state.hits.Peek().time > windowSeconds)
            state.hits.Dequeue();

        state.hits.Enqueue(new HitRecord { time = serverTime, headshot = isHeadshot });
        state.consecutiveHeadshots = isHeadshot ? state.consecutiveHeadshots + 1 : 0;
        state.lastHitTime = serverTime;

        int headshots = 0;
        foreach (HitRecord hit in state.hits)
            if (hit.headshot) headshots++;

        float ratio = state.hits.Count == 0 ? 0f : (float)headshots / state.hits.Count;
        bool suspicious = state.hits.Count >= minimumHits &&
            ratio >= suspiciousRatio && state.consecutiveHeadshots >= suspiciousStreak;

        return new Result
        {
            accepted = true,
            suspicious = suspicious,
            reason = suspicious
                ? "Unusually high headshot ratio and consecutive headshots; review required."
                : "Server-validated hit."
        };
    }

    private static Result Reject(string reason, bool suspicious)
    {
        return new Result { accepted = false, suspicious = suspicious, reason = reason };
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
