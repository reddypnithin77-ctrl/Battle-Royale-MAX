# Build the Unity training scene

The repository includes an Editor scene builder that creates a playable local greybox scene and a training-bot prefab.

## Requirements
- Open the project in a compatible Unity Editor with the Unity UI (uGUI) package available.
- Let Unity finish importing and compiling scripts.
- If Unity reports that `UnityEngine.UI` is missing, install the built-in Unity UI package through Package Manager.

## Generate the scene
1. Open the project in Unity.
2. In the top menu, choose **Tools → Battle Royale → Build Training Scene**.
3. Unity creates `Assets/Scenes/TrainingArena.unity` and `Assets/Prefabs/TrainingBot.prefab`.
4. Open the generated scene and press Play.
5. Use keyboard WASD and mouse input in the Editor. Mobile movement/look controls still require a Canvas with the existing joystick and button components wired as described in [Mobile HUD and loot](MOBILE_HUD_AND_LOOT.md).

The scene builder creates a capsule player, third-person camera, simple hitscan training weapon, greybox arena, safe-zone visual, pickups, six local practice bots, countdown/status label, on-screen mobile joystick, fire/jump/reload/weapon-switch buttons, and health/ammo/zone HUD. UI touches are excluded from right-side camera-look input. The bot prefab is a simple capsule placeholder.

This is a local prototype scene, not a finished game or APK. It has no online multiplayer, matchmaking, production character animations, polished art, or server-authoritative security. Test and adjust the scene in Unity before building for Android.
