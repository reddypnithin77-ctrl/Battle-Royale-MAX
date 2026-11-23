using System;
using UnityEngine;

// Prototype audit event model. A production service should persist these on the server.
[Serializable]
public class BRAuditEvent
{
    public string playerId;
    public string rule;
    public string detail;
    public long unixTime;
}

public class BRAuditReporter : MonoBehaviour
{
    public event Action<BRAuditEvent> EventReported;

    public void Report(string playerId, string rule, string detail)
    {
        BRAuditEvent auditEvent = new BRAuditEvent
        {
            playerId = string.IsNullOrEmpty(playerId) ? "unknown" : playerId,
            rule = string.IsNullOrEmpty(rule) ? "unspecified" : rule,
            detail = string.IsNullOrEmpty(detail) ? "" : detail,
            unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        Debug.LogWarning("[BR Anti-Cheat] " + auditEvent.rule + " | player=" +
            auditEvent.playerId + " | " + auditEvent.detail);
        EventReported?.Invoke(auditEvent);
    }
}
