# Unity Play Mode physics tests

The Unity Test Framework Play Mode suite is in `Assets/Tests/PlayMode`.

## Included checks

- CharacterController moves from the mobile joystick bridge
- Grounded jump produces upward movement
- Crouch smoothly reduces controller height
- Sprint covers more distance than walking
- Weapon firing consumes ammo and adds upward recoil
- Weapon reload transfers reserve ammo into the magazine
- Vehicle throttle accelerates its Rigidbody
- Vehicle braking reduces forward speed

## Run in Unity

1. Open the repository as a Unity project in Unity Hub.
2. Install/enable the **Test Framework** package if Unity reports that `UnityEngine.TestRunner` or NUnit is unavailable.
3. Open **Window → General → Test Runner**.
4. Select **PlayMode** and click **Run All**.
5. Review each test result and the Console for failures.

These tests exercise runtime components; they do not validate network multiplayer, vehicle suspension, Android device performance, or production collision tuning. The repository currently does not include a complete Unity project settings/package manifest, so it may need to be opened from a configured Unity project before the Test Runner can execute.
