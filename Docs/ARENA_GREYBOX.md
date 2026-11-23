# Greybox training arena

The `ProceduralTrainingArena` component creates a simple practice space at runtime using Unity cube primitives: a ground plane, boundary walls, cover blocks, and basic open-front buildings. It is intentionally a greybox layout; replace the primitives with original or properly licensed art before release.

## Scene setup
1. Create an empty GameObject named `ArenaGenerator` and add `ProceduralTrainingArena`.
2. Press Play. The arena is generated under that object. Adjust `arenaSize`, `coverCount`, `buildingCount`, and `randomSeed` in the Inspector.
3. Create a player spawn Transform and assign it to `playerSpawn` if you want the generator to position it.
4. For a moving practice dummy, create a capsule, add a CharacterController, BRTargetHealth, and TrainingBotPatrol. Position it on the ground.

This is runtime-generated greybox geometry, not a finished art map. The patrol dummy is local practice behavior only; there is no online AI, matchmaking, or multiplayer simulation.
