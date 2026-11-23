# Battle Royale MAX — Original prototype

An original battle-royale project. It is **not Free Fire MAX** and does not use Garena assets, characters, maps, branding, or proprietary code.

## Browser prototype
A small single-player browser prototype lives in `docs/index.html`. It demonstrates movement, aiming, firing at training bots, health, and a shrinking safe zone. It is not a full 3D game and does not connect real players.

GitHub Pages: https://reddypnithin77-ctrl.github.io/Battle-Royale-MAX/

## Unity Android starter
The `Assets/Scripts` folder includes starter components for mobile input, third-person movement/camera, raycast weapons, health, safe zones, pickups, loot, bots, and match flow. Read [Unity Android setup](Docs/UNITY_ANDROID_STARTER.md).

## Gameplay systems
- Mobile virtual joystick and touch buttons for fire, jump, reload, and weapon switching.
- Weapon loadout slots, pickups, ammo, and weighted loot spawning.
- Local training arena, bot spawning, and practice-round flow.
- Original physics upgrade: accelerated movement, sprint/crouch, weapon recoil/spread, and an arcade Rigidbody vehicle controller. See [Original gameplay physics](Docs/ORIGINAL_GAMEPLAY_PHYSICS.md).
- Unity Play Mode runtime tests for player movement/jump/crouch/sprint, weapon reload, and vehicle acceleration/braking. See [Play Mode physics tests](Docs/PLAY_MODE_PHYSICS_TESTS.md).
- Anti-cheat validation foundation and an in-memory authoritative match-state example. See [Anti-cheat foundation](Docs/ANTI_CHEAT_FOUNDATION.md) and [authoritative match service](Docs/AUTHORITATIVE_MATCH_SERVICE.md).

## Generate a playable Unity training scene
Unity Editor menu: **Tools → Battle Royale → Build Training Scene**. See [Build the training scene](Docs/BUILD_TRAINING_SCENE.md).

## Still required
- Original 3D player, map, vehicle, and weapon assets, animations, sounds, and UI prefabs
- Proper vehicle suspension and tuning, more advanced movement/animation integration
- Real multiplayer networking, matchmaking, authoritative servers, moderation, and security
- Unity compilation, device testing, performance optimization, Android signing, and release packaging

## Build note
Unity Editor and Android Build Support are required on a Windows, macOS, or Linux computer to assemble and build the Android app. The source scripts are not a finished APK and must be tested in Unity. Use only original or properly licensed assets.
