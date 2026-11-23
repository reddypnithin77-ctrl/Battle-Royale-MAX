# Player identity and login integration

## Implemented

- `Assets/Integrations/UnityAuthentication/BRUnityAuthenticationClient.cs`: Unity Authentication guest sign-in, username/password sign-up and sign-in, linking username/password to a guest account, and backend verification before the game receives an authenticated player ID.
- `Packages/manifest.json`: pins the Unity Authentication package to version 2.7.2 or later-compatible API baseline.
- `Server/src/server.js`: verifies Unity-signed access tokens using Unity's published JWKS, checks issuer, RS256 signature, expiry and configured project ID, and derives identity only from the verified `sub` claim.
- PostgreSQL-backed player profile records are created/updated after successful token verification.
- A basic per-process IP rate limit protects the verification endpoint.
- The Unity adapter is compile-gated until Unity Services packages and the scripting define are configured.

## Unity project setup (you must do this in your Unity account)

1. Open the project in a supported Unity Editor.
2. Install **Authentication** and **Services Core** using Package Manager.
3. Link the Unity project to your Unity Dashboard project and configure Authentication.
4. Add `BR_UNITY_AUTHENTICATION` to Player Settings > Other Settings > Scripting Define Symbols for the target platform.
5. Add `BRUnityAuthenticationClient` to a scene object and subscribe to its `Authenticated` and `SignInFailed` events.
6. Send the access token from the `Authenticated` event to your deployed backend over HTTPS in an `Authorization: Bearer <token>` header. Do not save or log the token.

The included method creates an anonymous guest account. Guest identity is not a recoverable account by itself. To support account recovery and sign-in on a second device, enable a supported external identity provider in the Unity Dashboard and implement the matching provider SDK/token-linking flow. Do not advertise account recovery until that flow has been configured and tested. Do not send provider passwords or tokens to anyone.

## Backend configuration

Requirements: Node.js 20+, PostgreSQL 14+, and HTTPS at the public edge.

Environment variables:
- `PORT`: optional, defaults to 8080.
- `UNITY_PROJECT_ID`: required; exact project ID from the Unity Dashboard.
- `DATABASE_URL`: required PostgreSQL connection string. Store it as a hosting secret, never in source control or the Unity client.
- `PGSSLMODE=disable`: local development only. Production should use TLS with a trusted database certificate.

Run locally from `Server/`:

```sh
npm install
UNITY_PROJECT_ID="your-unity-project-id" DATABASE_URL="postgresql://..." npm start
```

Endpoints:
- `GET /health`: checks database connectivity.
- `POST /auth/verify`: requires a Unity access token in the Authorization header. It creates or updates a profile row and returns the verified player ID plus timestamps.

The server creates the `player_profiles` table on startup. The ID is always taken from the verified token's `sub` claim; no client-supplied player ID is trusted.

## Deployment checklist

1. Create a private PostgreSQL database with automated backups.
2. Deploy the `Server/` directory to a Node.js 20+ host.
3. Set `UNITY_PROJECT_ID`, `DATABASE_URL`, and `PORT` as host environment variables/secrets.
4. Require HTTPS and configure the host's health check to use `/health`.
5. Configure platform-level rate limiting, request logging without authorization headers, alerts, and database backups.
6. Test invalid, expired, wrong-project, and valid Unity tokens before enabling the endpoint in the game client.

## Important limitations

- Username/password sign-up, sign-in, and linking are implemented in the Unity adapter but require the owner to enable the provider in Unity Dashboard and wire actual scene UI. Password reset/support is not implemented.
- The in-process rate limiter resets when the service restarts and is not shared between multiple instances. Use gateway-level or shared-store rate limiting for public production traffic.
- This is an identity/profile starter, not a full game backend. It does not implement matchmaking, authoritative network simulation, persistent inventory, password reset, moderation, or account support tooling.
- The Package Manager manifest now declares Authentication 2.7.2, but the repository still lacks ProjectSettings/ProjectVersion.txt and a complete Unity scene/build configuration. The project owner must choose the matching Unity Editor version and configure Android Build Support before an Android build can be verified.
- Do not put Unity Service Account secrets, database credentials, or signing keys in the client APK.
