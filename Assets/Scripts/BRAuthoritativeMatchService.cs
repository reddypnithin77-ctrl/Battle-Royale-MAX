using System.Collections.Generic;
using UnityEngine;

// In-memory authoritative match rules. Invoke ONLY from trusted server code after
// authenticating the session; never expose these methods as client-trusted commands.
public class BRAuthoritativeMatchService
{
    private class PlayerState
    {
        public Vector3 position;
        public float lastMoveTime;
        public float lastShotTime = -999f;
        public int ammo;
        public int reserveAmmo;
        public int health;
        public bool reloading;
        public float maximumSpeed;
        public bool eliminated;
    }

    private readonly Dictionary<string, PlayerState> players = new Dictionary<string, PlayerState>();
    private readonly BRAuditReporter audit;

    public BRAuthoritativeMatchService(BRAuditReporter auditReporter)
    {
        audit = auditReporter;
    }

    public bool RegisterPlayer(string authenticatedPlayerId, Vector3 spawnPosition, float serverTime,
        float maximumSpeed = 5.5f, int magazineAmmo = 30, int reserveAmmo = 90, int health = 100)
    {
        if (string.IsNullOrWhiteSpace(authenticatedPlayerId) || players.ContainsKey(authenticatedPlayerId) ||
            maximumSpeed <= 0f || magazineAmmo < 0 || reserveAmmo < 0 || health <= 0)
            return false;

        players.Add(authenticatedPlayerId, new PlayerState
        {
            position = spawnPosition,
            lastMoveTime = serverTime,
            maximumSpeed = maximumSpeed,
            ammo = magazineAmmo,
            reserveAmmo = reserveAmmo,
            health = health
        });
        return true;
    }

    public bool TryMove(string authenticatedPlayerId, Vector3 proposedPosition, float serverTime,
        float movementTolerance = 0.35f, float maximumVerticalDelta = 1.8f)
    {
        PlayerState state;
        if (!TryGetActivePlayer(authenticatedPlayerId, out state)) return false;

        float elapsed = serverTime - state.lastMoveTime;
        bool valid = BRServerValidationRules.IsMovementValid(
            state.position, proposedPosition, elapsed, state.maximumSpeed,
            movementTolerance, maximumVerticalDelta);

        if (!valid)
        {
            Report(authenticatedPlayerId, "movement", "Rejected movement update");
            return false;
        }

        state.position = proposedPosition;
        state.lastMoveTime = serverTime;
        return true;
    }

    public bool TryFire(string authenticatedPlayerId, float serverTime, float shotsPerSecond,
        float damage, float maximumDamage)
    {
        PlayerState state;
        if (!TryGetActivePlayer(authenticatedPlayerId, out state)) return false;

        if (!BRServerValidationRules.IsShotValid(serverTime, state.lastShotTime,
            shotsPerSecond, state.ammo, state.reloading) ||
            !BRServerValidationRules.IsDamageValid(damage, maximumDamage))
        {
            Report(authenticatedPlayerId, "weapon", "Rejected invalid fire request");
            return false;
        }

        state.lastShotTime = serverTime;
        state.ammo--;
        return true;
    }

    public bool TryCollectAmmo(string authenticatedPlayerId, Vector3 serverPickupPosition,
        float pickupDistance, int ammoAmount, int maximumReserveAmmo)
    {
        PlayerState state;
        if (!TryGetActivePlayer(authenticatedPlayerId, out state)) return false;

        if (!BRServerValidationRules.IsPickupValid(state.position, serverPickupPosition, pickupDistance) ||
            !BRServerValidationRules.IsResourceChangeValid(state.reserveAmmo, ammoAmount, maximumReserveAmmo))
        {
            Report(authenticatedPlayerId, "pickup", "Rejected invalid ammo pickup");
            return false;
        }

        state.reserveAmmo += ammoAmount;
        return true;
    }

    public bool TrySetReloading(string authenticatedPlayerId, bool reloading)
    {
        PlayerState state;
        if (!TryGetActivePlayer(authenticatedPlayerId, out state)) return false;
        if (reloading && (state.ammo >= 30 || state.reserveAmmo <= 0)) return false;
        state.reloading = reloading;
        return true;
    }

    public bool TryApplyServerDamage(string authenticatedPlayerId, int damage)
    {
        PlayerState state;
        if (!TryGetActivePlayer(authenticatedPlayerId, out state) || damage <= 0 || damage > 100) return false;
        state.health = Mathf.Max(0, state.health - damage);
        if (state.health == 0) state.eliminated = true;
        return true;
    }

    public bool TryGetSnapshot(string authenticatedPlayerId, out Vector3 position,
        out int health, out int magazineAmmo, out int reserveAmmo, out bool eliminated)
    {
        PlayerState state;
        if (!players.TryGetValue(authenticatedPlayerId, out state))
        {
            position = Vector3.zero;
            health = magazineAmmo = reserveAmmo = 0;
            eliminated = true;
            return false;
        }

        position = state.position;
        health = state.health;
        magazineAmmo = state.ammo;
        reserveAmmo = state.reserveAmmo;
        eliminated = state.eliminated;
        return true;
    }

    private bool TryGetActivePlayer(string playerId, out PlayerState state)
    {
        if (players.TryGetValue(playerId, out state) && !state.eliminated) return true;
        state = null;
        return false;
    }

    private void Report(string playerId, string rule, string detail)
    {
        if (audit != null) audit.Report(playerId, rule, detail);
    }
}
