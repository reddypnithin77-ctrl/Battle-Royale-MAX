# Practice match setup

This stage adds a local practice-match loop and a bot spawner. It is not online multiplayer.

## Bot spawner
1. Make a bot prefab with a visible model, Collider, CharacterController, BRTargetHealth, and optionally TrainingBotPatrol.
2. Add BRTrainingBotSpawner to an empty scene object.
3. Assign the prefab and several Transform spawn points. Set botCount and spawnInterval.

## Match flow
1. Add BRMatchFlow to a scene manager object.
2. Assign the player's Health component and an optional legacy UI Text for status.
3. When the countdown finishes, the practice round begins. Defeating all objects with BRTargetHealth shows "PRACTICE COMPLETE!". If the player's Health reaches zero, it shows "YOU WERE ELIMINATED".
4. Place only target-health components on practice bots, not on unrelated props.

The flow uses local scene objects and periodic scans. A real multiplayer match needs server-authoritative player state, networking, matchmaking, latency handling, anti-cheat, and moderation; none is included here.
