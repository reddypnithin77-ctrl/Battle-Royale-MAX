# Unity Android starter

This repository is an original battle-royale prototype, not a copy of Free Fire MAX. It contains no Garena source code, extracted assets, characters, maps, or branding.

## Added starter systems
- Mobile input bridge for joystick movement, fire, jump, and right-side touch camera look.
- Third-person character movement using Unity CharacterController.
- Orbiting third-person camera.
- Basic raycast weapon and reusable target health component.

These scripts are a foundation, not a finished game or APK. A Unity scene, original 3D models, animations, UI prefabs, audio, optimization, testing, and any multiplayer backend still need to be built.

## Setup
1. Install Unity LTS on a Windows, macOS, or Linux computer through Unity Hub and add Android Build Support (SDK/NDK and OpenJDK).
2. Open this repository as a Unity project. If Unity reports missing project metadata, create a new 3D project and copy the Assets folder into it.
3. Create a ground plane and capsule player. Add CharacterController and BRPlayerMotor to the player.
4. Add a camera with BRThirdPersonCamera; assign the player as target and assign the camera to BRPlayerMotor.cameraTransform.
5. Add MobileInputBridge to an empty scene object.
6. Add BRHitscanWeapon to the player or weapon object; add BRTargetHealth to practice targets.
7. Build Canvas controls. Connect joystick output to SetMove(Vector2), fire pointer-down/up to SetFire(true/false), and jump button to PressJump().
8. Test in Editor, then select Android in File > Build Profiles and build to your device.

## Limitations
- Touch look uses touches on the right side of the screen; keep UI buttons out of that area.
- No online multiplayer, matchmaking, accounts, inventory, parachute, vehicles, anti-cheat, or production map is included yet.
- Use original or properly licensed assets only.
