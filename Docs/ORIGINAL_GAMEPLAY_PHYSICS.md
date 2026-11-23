# Original gameplay physics upgrade

This upgrade adds reusable Unity gameplay components. It is original code and does not import or reproduce Free Fire MAX internals.

## Player movement
`BRPlayerMotor` now uses acceleration, air-control scaling, sprinting (Left Shift), crouching (C), jump input, and smoothed CharacterController stance height. Mobile movement continues to use `MobileInputBridge`.

Recommended starting values:
- Walk: 5.5 m/s; sprint: 8 m/s; crouch: 2.8 m/s
- Gravity: -22 m/s²; jump height: 1.2 m
- CharacterController height: about 1.9 m standing and 1.15 m crouched

Ensure the CharacterController's center and capsule dimensions fit your original player model. Add a ceiling-clearance check before enabling crouch-to-stand in production.

## Weapon ballistics
`BRHitscanWeapon` now adds configurable random angular spread, extra spread while moving, per-shot pitch/yaw recoil values, and recoil recovery. `CurrentRecoil` exposes the current recoil amount for a camera/weapon animation script; this script does not automatically rotate the camera.

## Vehicle physics
Add a Rigidbody and collider to a vehicle root, then attach `BRVehicleController`. Assign a low center-of-mass Transform for stability. Drive it with:
- `SetDriveInput(-1..1)`
- `SetSteeringInput(-1..1)`
- `SetBrake(true/false)`

Use a non-trigger collider and tune mass, drag, angular drag, collider shape, and suspension separately for each vehicle. This is an arcade Rigidbody controller, not a complete wheel/suspension simulation. For production vehicles, use WheelColliders or a custom suspension model, plus server-authoritative validation in multiplayer.

## Important limitations
These scripts have been committed as source only. They have not been compiled or play-tested in Unity in this environment. They do not create vehicle models, animations, sound, multiplayer replication, or a finished APK. Test and tune them in Unity on target Android devices.
