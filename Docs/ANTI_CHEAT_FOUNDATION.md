# Anti-cheat foundation

This adds reusable validation rules and an audit-event model for an original game. It does **not** copy Free Fire MAX's private anti-cheat system, and it does not make this offline prototype cheat-proof.

## Included rules
- Movement distance versus elapsed server time and configured movement speed.
- Server fire-rate, ammunition, and reload-state checks.
- Damage bounds.
- Pickup distance checks using server-known positions.
- Resource/inventory bounds.
- Structured audit events for suspicious actions.

## Critical integration rule
These checks only provide meaningful protection when called by a trusted authoritative server. The server must own player positions, health, ammo, reload timers, pickups, hit validation, and match outcomes. Do not accept a client's claim that it hit a target, collected an item, changed its ammo, or moved to a position.

The current Unity project is an offline/local prototype. Calling these checks from a client-only MonoBehaviour would not stop a modified client; a production multiplayer backend and authenticated networking layer are still required.

## Recommended response policy
1. Reject an invalid action first; do not immediately ban based on one signal.
2. Record rule name, server timestamp, player/session identifier, and relevant server-side context.
3. Review repeated independent violations and allow for latency, packet loss, and accessibility/device differences.
4. Apply graduated, reviewable actions. Keep an appeal/review process.
5. Protect audit logs and avoid collecting unrelated personal/device data.

## Example
A server handling a movement update should compare the proposed position against the last server-approved position, server-measured elapsed time, and the player's server-assigned movement speed using `BRServerValidationRules.IsMovementValid`. Only then should it update the authoritative position.
