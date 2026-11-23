# Authoritative match service integration

`BRAuthoritativeMatchService` is an in-memory example of how a trusted server can own player state.

## What it enforces
- Authenticated player IDs are registered once.
- Player positions are advanced only after movement validation against server time and server-assigned speed.
- Fire requests consume server-held ammunition and obey server fire cadence/reload state.
- Ammo pickups use server-known player and pickup positions plus inventory caps.
- Server-applied damage controls health and elimination.
- A limited snapshot API exposes current authoritative state to a server transport layer.

## Integration boundary
This class is deliberately transport-agnostic. It is not a network server, authentication provider, matchmaking service, or persistence layer. A production backend must:
1. Authenticate each connection and derive player IDs from the authenticated session, never from an arbitrary client string.
2. Call this service only from trusted server handlers.
3. Keep pickup locations, weapon stats, damage results, time, and movement limits on the server.
4. Perform server-side ray/physics hit validation before applying damage.
5. Replicate approved snapshots/events to clients and rate-limit incoming requests.
6. Persist audit events and match outcomes securely.

Do not connect client RPCs directly to these methods without server authentication and validation. The current game remains an offline Unity prototype; no online server has been deployed or tested.
