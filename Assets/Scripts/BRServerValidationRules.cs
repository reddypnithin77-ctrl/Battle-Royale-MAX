using UnityEngine;

// Deterministic rules intended to run on a trusted game server.
// Never trust a client-provided position, hit result, ammo count, or damage value.
public static class BRServerValidationRules
{
    public static bool IsMovementValid(
        Vector3 previousPosition,
        Vector3 proposedPosition,
        float elapsedSeconds,
        float maximumSpeed,
        float tolerance,
        float maximumVerticalDelta)
    {
        if (!IsFinite(elapsedSeconds) || !IsFinite(maximumSpeed) || !IsFinite(tolerance) ||
            !IsFinite(maximumVerticalDelta) || !IsFinite(previousPosition.x) || !IsFinite(previousPosition.y) ||
            !IsFinite(previousPosition.z) || !IsFinite(proposedPosition.x) || !IsFinite(proposedPosition.y) ||
            !IsFinite(proposedPosition.z) || elapsedSeconds <= 0f || maximumSpeed < 0f ||
            tolerance < 0f || maximumVerticalDelta < 0f) return false;
        Vector3 delta = proposedPosition - previousPosition;
        if (Mathf.Abs(delta.y) > maximumVerticalDelta + tolerance) return false;
        delta.y = 0f;
        float allowedDistance = maximumSpeed * elapsedSeconds + Mathf.Max(0f, tolerance);
        return delta.sqrMagnitude <= allowedDistance * allowedDistance;
    }

    public static bool IsShotValid(
        float serverTime,
        float previousAcceptedShotTime,
        float shotsPerSecond,
        int serverMagazineAmmo,
        bool serverIsReloading)
    {
        if (!IsFinite(serverTime) || !IsFinite(previousAcceptedShotTime) || !IsFinite(shotsPerSecond) ||
            serverMagazineAmmo <= 0 || serverIsReloading || shotsPerSecond <= 0f ||
            serverTime < previousAcceptedShotTime) return false;
        float minimumInterval = 1f / shotsPerSecond;
        return serverTime - previousAcceptedShotTime >= minimumInterval;
    }

    public static bool IsDamageValid(float damage, float maximumAllowedDamage)
    {
        return IsFinite(damage) && IsFinite(maximumAllowedDamage) &&
            damage > 0f && maximumAllowedDamage > 0f && damage <= maximumAllowedDamage;
    }

    public static bool IsPickupValid(
        Vector3 serverPlayerPosition,
        Vector3 serverPickupPosition,
        float allowedDistance)
    {
        if (!IsFinite(allowedDistance) || !IsFinite(serverPlayerPosition.x) || !IsFinite(serverPlayerPosition.y) ||
            !IsFinite(serverPlayerPosition.z) || !IsFinite(serverPickupPosition.x) ||
            !IsFinite(serverPickupPosition.y) || !IsFinite(serverPickupPosition.z) || allowedDistance <= 0f) return false;
        return (serverPlayerPosition - serverPickupPosition).sqrMagnitude <= allowedDistance * allowedDistance;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    public static bool IsResourceChangeValid(int currentAmount, int requestedChange, int maximumAmount)
    {
        if (currentAmount < 0 || maximumAmount < 0) return false;
        long result = (long)currentAmount + requestedChange;
        return result >= 0 && result <= maximumAmount;
    }
}
