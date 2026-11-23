# Mobile HUD, weapon slots, and loot

## Mobile controls
1. Add an EventSystem and Canvas to the scene.
2. Create a joystick background UI Image in the lower-left corner, with a child handle Image. Add `BRVirtualJoystick` to the background and assign the handle.
3. Add UI Buttons for Fire, Jump, Reload, Next Weapon, and Previous Weapon. Add `BRMobileButton` to each and select its action in the Inspector. Assign the player's `BRWeaponLoadout` to weapon buttons.
4. Keep the right half of the screen clear for camera-look touch input. Ensure the Canvas has a GraphicRaycaster and the scene has an EventSystem.

## Weapon slots
1. Parent each weapon GameObject under the player and add `BRHitscanWeapon` to each weapon object.
2. Add `BRWeaponLoadout` to the player and populate `weaponSlots` in the Inspector.
3. Set the starting slot. The component activates one weapon at a time; Next/Previous buttons cycle through assigned slots.
4. Place a pickup prefab with a trigger collider and `BRWeaponPickup` in the map. Set its slot index to match the player's loadout.

## Loot spawning
1. Create pickup prefabs (for example ammo or health pickups) using original art and trigger colliders.
2. Add `BRLootSpawner` to an empty scene object.
3. Create empty GameObjects at intended loot locations and assign them to `spawnPoints`.
4. Add prefab entries and weights to `lootTable`; set each entry's prefab, weight, and intended max count. (The current simple spawner uses weights; maxActive is reserved for a future cap implementation.)
5. Press Play to spawn one weighted pickup at each assigned point. Collected/destroyed items respawn after the configured delay.

All systems are local prototype components. They do not implement network synchronization, anti-cheat, or multiplayer inventory replication.
