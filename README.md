# INDIAN FIRE — Original battle-royale prototype

An original battle-royale project with a fictional India-inspired world. It is **not Free Fire MAX** and does not use Garena assets, characters, maps, branding, or proprietary code.

## Browser prototype
A small single-player browser prototype lives in `docs/index.html`. It demonstrates movement, aiming, firing at training bots, health, and a shrinking safe zone. It is not a full 3D game and does not connect real players.

GitHub Pages: https://reddypnithin77-ctrl.github.io/Battle-Royale-MAX/

## Unity Android starter
The `Assets/Scripts` folder includes starter components for mobile input, third-person movement/camera, raycast weapons, health, safe zones, pickups, loot, bots, and match flow. Read [Unity Android setup](Docs/UNITY_ANDROID_STARTER.md).

## INDIAN FIRE world
The procedural world builder creates a full, stylized India-shaped mainland layout, including the Northeast extension and southern peninsula, with named districts, simple enterable greybox buildings, connecting roads, loot-area labels, health supplies, and ammo supplies. The structures are prototype geometry intended to be replaced with optimized original art.

Named districts include Snowcrest Gate, Victory Arch, Moonstone Palace, Liongate Port, Four Arch Bazaar, River Crown, Golden Sanctuary, Brahmaputra Frontier, Eastern Highlands, Sunspire Coast, Tech Valley, Lotus Reach, and Backwater Haven.

## Generate a playable Unity scene
1. Open the project in Unity Editor.
2. Wait for scripts to compile.
3. Use **Tools → INDIAN FIRE → Build World Scene**.
4. Open the generated `Assets/Scenes/TrainingArena.unity` scene and press Play.

See [INDIAN FIRE map guide](Docs/ARENA_GREYBOX.md).

## Gameplay systems
- Landscape-first mobile play with a virtual joystick and touch buttons for fire, jump, reload, and weapon switching.
- Weapon loadout slots, pickups, ammo, and weighted loot spawning.
- Local training bots, safe zone, health, and practice-round flow.
- Original movement physics, vehicle prototype, anti-cheat validation foundation, and an in-memory authoritative match-state example.

## Unity project setup note
The repository currently does not include `ProjectSettings/ProjectVersion.txt` or the rest of Unity's `ProjectSettings` folder. Unity Editor version and project settings must be established on the development machine before a reliable editor compile or Android build can be verified. Do not guess a Unity version; use the version installed for the project and resolve package compatibility in Package Manager.

## Still required
- Original high-quality 3D characters, buildings, terrain, vehicles, animations, sound, and UI assets
- Real multiplayer networking, matchmaking, authoritative servers, moderation, and security
- Unity compilation, device testing, performance optimization, Android signing, and release packaging

## Build note
Unity Editor and Android Build Support are required on a Windows, macOS, or Linux computer to assemble and build the Android app. The source scripts are not a finished APK and must be tested in Unity. Use only original or properly licensed assets.
