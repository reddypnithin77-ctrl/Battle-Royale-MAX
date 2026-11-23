# Battle Royale MAX — Unity Android Starter

An original battle-royale prototype starter. It is **not Free Fire MAX** and does not contain Garena assets, maps, characters, branding, or code.

## Current scope
This is a code starter, not a finished game or APK. It provides a basic third-person controller, camera, safe-zone prototype, health component, and project setup notes. The first milestone is a local playable prototype. Real online multiplayer comes after the movement/combat loop is tested.

## Engine
- Unity 6 (3D Core)
- Android Build Support (SDK/NDK + OpenJDK installed through Unity Hub)
- C# scripts in `Assets/Scripts`

## Setup
1. Create a new Unity 6 3D project.
2. Copy `Assets/` from this package into the project.
3. Create a Plane named `Ground` and add a Collider.
4. Create a Capsule named `Player`; add a `CharacterController`; attach `PlayerController.cs` and `Health.cs`.
5. Add a Camera and attach `ThirdPersonCamera.cs`; assign the Player transform to `target`.
6. Create an empty object named `SafeZone`; attach `SafeZone.cs`; assign a visible cylinder or ring as `visual`.
7. Press Play. Desktop keyboard controls are WASD + mouse; Android touch UI is a later milestone.

## Android build
In Unity: File → Build Profiles → Android → Switch Platform. Set a unique application identifier, choose ARM64, then Build. This package does not include a signed release APK.

## Multiplayer plan
Do not trust client-reported hits, health, loot, or match results. The production version needs a server-authoritative session service:
- matchmaking and room allocation
- server-owned movement validation and combat resolution
- replicated player state
- disconnect/reconnect handling
- moderation, reporting, and rate limits

Recommended next milestone: integrate a multiplayer SDK/backend after the local controller and safe-zone loop work reliably. A 50-player match needs dedicated server capacity, load testing, and operational monitoring; it is not included here.
