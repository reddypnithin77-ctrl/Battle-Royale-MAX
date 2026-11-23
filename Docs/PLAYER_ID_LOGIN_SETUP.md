# Player identity and login integration

## What is included

- `Assets/Integrations/UnityAuthentication/BRUnityAuthenticationClient.cs` is a Unity Authentication guest sign-in adapter.
- `Server/src/server.js` verifies a Unity Authentication bearer token using Unity's published JWKS and returns the player ID from the verified `sub` claim.
- The client adapter is behind the `BR_UNITY_AUTHENTICATION` scripting define so a clean checkout without Unity Services packages does not fail to compile.

## Unity setup

1. Open the project in Unity and install **Authentication** and **Services Core** using Package Manager.
2. Configure the Unity project in the Unity Dashboard and link the project to its Unity Project ID.
3. Add `BR_UNITY_AUTHENTICATION` to Player Settings > Other Settings > Scripting Define Symbols for the target platform.
4. Add `BRUnityAuthenticationClient` to a scene object and subscribe to its `Authenticated` and `SignInFailed` events.
5. Send the returned access token to your own backend over HTTPS as an `Authorization: Bearer <token>` header. Do not save or log the token.

The included client method is guest sign-in. Unity documents that anonymous accounts cannot be recovered on another device unless linked to an external identity provider; uninstalling the app can also lose an unlinked guest account. Add a supported external provider before treating this as a production account system.

## Backend setup

1. Install Node.js 20 or newer.
2. In this folder run `npm install`.
3. Set `UNITY_PROJECT_ID` to the exact Project ID from the Unity Dashboard.
4. Start with `npm start`.
5. Deploy behind HTTPS and expose `POST /auth/verify` to the game client.

The verifier fetches Unity's public signing keys from `https://player-auth.services.api.unity.com/.well-known/jwks.json`, verifies the RS256 signature and issuer, checks token expiry through the JWT library, checks `project_id`, and takes the account ID only from the verified `sub` claim. JWKS keys are cached and refreshed by the JOSE remote key-set helper.

## Important boundary

This is an authentication verification starter, not a complete production backend. It does not yet provide persistent profiles, password/email login, external-account linking, account recovery, database storage, rate limiting, match hosting, or server-authoritative physics. Never trust a player ID sent in a JSON body, and never put Unity Service Account credentials in the game client. Add deployment-level rate limiting, monitoring, database authorization, and abuse controls before public release.
