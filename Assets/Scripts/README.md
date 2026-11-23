## Scene wiring
- Player: Capsule + CharacterController + PlayerController + Health.
- Main Camera: ThirdPersonCamera; set `target` to Player.
- SafeZone: Empty GameObject + SafeZone. Assign a flat cylinder/ring to `visual`.
- Ground: Plane with collider.

The starter uses Unity's legacy input axes for quick desktop testing. For a real Android release, migrate movement and camera look to the Unity Input System and add touch UI. Safe-zone damage is intentionally local-only; do not use client-side health as an authority in an online game.
