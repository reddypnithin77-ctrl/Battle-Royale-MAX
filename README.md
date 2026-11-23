# Battle Royale MAX — Original prototype

An original battle-royale project. It is **not Free Fire MAX** and does not use Garena assets, characters, maps, branding, or proprietary code.

## Browser prototype
A small single-player browser prototype lives in `docs/index.html`. It demonstrates movement, aiming, firing at training bots, health, and a shrinking safe zone. It is not a full 3D game and does not connect real players.

GitHub Pages: https://reddypnithin77-ctrl.github.io/Battle-Royale-MAX/

## Unity Android starter
The `Assets/Scripts` folder now includes starter components for:
- Mobile input bridge (virtual joystick values, fire, jump, and right-side touch look)
- Third-person character movement and follow camera
- Raycast weapon and target health
- Safe-zone shrinking and outside-zone damage
- Collectible health pickups

Read [Unity Android setup](Docs/UNITY_ANDROID_STARTER.md) and [project roadmap](Docs/ROADMAP.md). These scripts are a foundation, not a complete game or installable APK.

## Latest gameplay additions
- Mobile virtual joystick and touch buttons for fire, jump, reload, and weapon switching.
- Weapon loadout slots and weapon pickups.
- Weighted loot spawning at hand-placed map points, with respawn after pickup.
- Setup notes: [Mobile HUD and loot guide](Docs/MOBILE_HUD_AND_LOOT.md).

## Practice match stage
- Local bot spawning from configured scene points.
- Countdown and practice-round status: training complete or player eliminated.
- Setup guide: [Practice match setup](Docs/PRACTICE_MATCH_SETUP.md).

## Still required
- A Unity scene, original 3D player/map/weapon assets, animations, and UI prefabs
- Loot/inventory, vehicles, parachute, bots and match flow
- Multiplayer networking, matchmaking, authoritative servers, moderation and security
- Device testing, performance optimization, Android signing and release packaging

## Build note
Unity Editor and Android Build Support are required on a Windows, macOS, or Linux computer to assemble and build the Android app. Use only original or properly licensed assets.


## Generate a playable Unity training scene
- Unity Editor menu: **Tools → Battle Royale → Build Training Scene**.
- Creates a local arena, player, third-person camera, placeholder weapon, pickups, safe-zone visual, bot prefab, and practice-match HUD.
- Guide: [Build the training scene](Docs/BUILD_TRAINING_SCENE.md).


## Anti-cheat foundation
- Reusable server-side validation for movement, firing cadence, ammo/reload state, damage, pickups, and resource bounds.
- Structured audit events for suspicious actions.
- Guide: [Anti-cheat foundation](Docs/ANTI_CHEAT_FOUNDATION.md).
