using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BRGameplayPhysicsPlayModeTests
{
    private GameObject bridgeObject;
    private GameObject floorObject;
    private GameObject playerObject;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        bridgeObject = new GameObject("PlayMode Test Input Bridge");
        bridgeObject.AddComponent<MobileInputBridge>();

        floorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorObject.name = "PlayMode Test Floor";
        floorObject.transform.position = new Vector3(0f, -0.5f, 0f);
        floorObject.transform.localScale = new Vector3(30f, 1f, 30f);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (bridgeObject != null) Object.Destroy(bridgeObject);
        if (floorObject != null) Object.Destroy(floorObject);
        if (playerObject != null) Object.Destroy(playerObject);
        yield return null;
    }

    private BRPlayerMotor CreatePlayer()
    {
        playerObject = new GameObject("PlayMode Test Player");
        GameObject player = playerObject;
        player.transform.position = new Vector3(0f, 1.05f, 0f);
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.4f;
        controller.center = new Vector3(0f, 1f, 0f);
        BRPlayerMotor motor = player.AddComponent<BRPlayerMotor>();
        motor.cameraTransform = player.transform;
        return motor;
    }

    [UnityTest]
    public IEnumerator PlayerMovesForwardFromMobileJoystick()
    {
        BRPlayerMotor motor = CreatePlayer();
        MobileInputBridge.Instance.SetMove(Vector2.up);

        yield return new WaitForSeconds(0.3f);

        Assert.Greater(motor.transform.position.z, 0.15f,
            "Joystick forward input should move the CharacterController.");
    }

    [UnityTest]
    public IEnumerator PlayerJumpRaisesCharacter()
    {
        BRPlayerMotor motor = CreatePlayer();
        yield return new WaitForSeconds(0.1f);
        float startY = motor.transform.position.y;

        MobileInputBridge.Instance.PressJump();
        yield return new WaitForSeconds(0.12f);

        Assert.Greater(motor.transform.position.y, startY + 0.1f,
            "A grounded jump input should produce upward movement.");
    }

    [UnityTest]
    public IEnumerator CrouchReducesCharacterControllerHeight()
    {
        BRPlayerMotor motor = CreatePlayer();
        yield return new WaitForSeconds(0.05f);
        float standingHeight = motor.GetComponent<CharacterController>().height;

        motor.SetCrouch(true);
        yield return new WaitForSeconds(0.25f);

        Assert.Less(motor.GetComponent<CharacterController>().height, standingHeight - 0.2f,
            "Crouch should smoothly reduce the controller height.");
    }

    [UnityTest]
    public IEnumerator SprintMovesFasterThanWalking()
    {
        BRPlayerMotor motor = CreatePlayer();
        MobileInputBridge.Instance.SetMove(Vector2.up);
        yield return new WaitForSeconds(0.35f);
        float walkDistance = motor.transform.position.z;

        motor.transform.position = new Vector3(0f, 1.05f, 0f);
        motor.SetSprint(true);
        yield return new WaitForSeconds(0.35f);
        float sprintDistance = motor.transform.position.z;

        Assert.Greater(sprintDistance, walkDistance * 1.15f,
            "Sprint should cover more distance than walking over the same interval.");
    }

    [UnityTest]
    public IEnumerator WeaponReloadTransfersReserveAmmo()
    {
        GameObject weaponObject = new GameObject("PlayMode Test Weapon");
        weaponObject.SetActive(false);
        BRHitscanWeapon weapon = weaponObject.AddComponent<BRHitscanWeapon>();
        weapon.magazineSize = 4;
        weapon.reserveAmmo = 12;
        weapon.reloadSeconds = 0.05f;
        weaponObject.SetActive(true);

        Assert.AreEqual(4, weapon.AmmoInMagazine);
        weapon.magazineSize = 10;
        weapon.Reload();
        yield return new WaitForSeconds(0.12f);

        Assert.AreEqual(10, weapon.AmmoInMagazine);
        Assert.AreEqual(6, weapon.ReserveAmmo);
        Object.Destroy(weaponObject);
    }

    [UnityTest]
    public IEnumerator VehicleThrottleAcceleratesRigidbody()
    {
        GameObject vehicle = new GameObject("PlayMode Test Vehicle");
        vehicle.transform.position = new Vector3(5f, 0.6f, 0f);
        vehicle.AddComponent<BoxCollider>();
        Rigidbody body = vehicle.AddComponent<Rigidbody>();
        body.useGravity = false;
        BRVehicleController controller = vehicle.AddComponent<BRVehicleController>();

        controller.SetDriveInput(1f);
        yield return new WaitForSeconds(0.35f);

        Assert.Greater(body.velocity.z, 0.5f,
            "Forward throttle should accelerate the vehicle Rigidbody.");
        Object.Destroy(vehicle);
    }

    [UnityTest]
    public IEnumerator VehicleBrakeReducesForwardSpeed()
    {
        GameObject vehicle = new GameObject("PlayMode Test Braking Vehicle");
        vehicle.transform.position = new Vector3(5f, 0.6f, 0f);
        vehicle.AddComponent<BoxCollider>();
        Rigidbody body = vehicle.AddComponent<Rigidbody>();
        body.useGravity = false;
        BRVehicleController controller = vehicle.AddComponent<BRVehicleController>();

        controller.SetDriveInput(1f);
        yield return new WaitForSeconds(0.35f);
        float speedBeforeBrake = body.velocity.magnitude;
        controller.SetDriveInput(0f);
        controller.SetBrake(true);
        yield return new WaitForSeconds(0.25f);

        Assert.Less(body.velocity.magnitude, speedBeforeBrake,
            "Brake input should reduce vehicle speed.");
        Object.Destroy(vehicle);
    }
}