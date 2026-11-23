#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BRTrainingSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/TrainingArena.unity";
    private const string PrefabPath = "Assets/Prefabs/TrainingBot.prefab";

    [MenuItem("Tools/Battle Royale/Build Training Scene")]
    public static void Build()
    {
        EnsureFolder("Assets/Scenes");
        EnsureFolder("Assets/Prefabs");
        GameObject botPrefab = CreateBotPrefab();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.58f);
        CreateLight();
        CreateArena();

        GameObject input = new GameObject("Mobile Input Bridge");
        input.AddComponent<MobileInputBridge>();

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.1f, -8f);
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.42f;
        controller.center = new Vector3(0f, 1f, 0f);
        player.AddComponent<Health>();
        BRPlayerMotor motor = player.AddComponent<BRPlayerMotor>();
        BRWeaponLoadout loadout = player.AddComponent<BRWeaponLoadout>();

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        BRThirdPersonCamera follow = cameraObject.AddComponent<BRThirdPersonCamera>();
        follow.target = player.transform;
        follow.offset = new Vector3(0f, 2.2f, -5.2f);
        motor.cameraTransform = cameraObject.transform;

        GameObject weapon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        weapon.name = "Training Rifle";
        weapon.transform.SetParent(player.transform);
        weapon.transform.localPosition = new Vector3(0.42f, 1.15f, 0.35f);
        weapon.transform.localScale = new Vector3(0.14f, 0.14f, 0.65f);
        Object.DestroyImmediate(weapon.GetComponent<BoxCollider>());
        BRHitscanWeapon gun = weapon.AddComponent<BRHitscanWeapon>();
        gun.aimCamera = camera;
        loadout.weaponSlots = new[] { weapon };

        CreatePickup("Health Pickup", PrimitiveType.Sphere, new Vector3(-4f, 0.6f, -1f), typeof(HealthPickup));
        CreatePickup("Ammo Pickup", PrimitiveType.Cube, new Vector3(4f, 0.6f, -1f), typeof(AmmoPickup));

        GameObject zone = new GameObject("Safe Zone");
        zone.transform.position = Vector3.zero;
        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Safe Zone Visual";
        disc.transform.SetParent(zone.transform);
        disc.transform.localPosition = new Vector3(0f, -0.92f, 0f);
        disc.transform.localScale = new Vector3(360f, 0.025f, 360f);
        Collider discCollider = disc.GetComponent<Collider>();
        if (discCollider != null) Object.DestroyImmediate(discCollider);
        SafeZone safe = zone.AddComponent<SafeZone>();
        safe.visual = disc.transform;
        safe.startRadius = 180f;
        safe.endRadius = 35f;
        safe.startDelay = 45f;
        safe.shrinkDuration = 360f;

        GameObject manager = new GameObject("Practice Match Manager");
        BRTrainingBotSpawner spawner = manager.AddComponent<BRTrainingBotSpawner>();
        spawner.botPrefab = botPrefab;
        spawner.botCount = 6;
        spawner.spawnInterval = 0.8f;
        spawner.spawnPoints = CreateSpawnPoints();
        BRMatchFlow match = manager.AddComponent<BRMatchFlow>();
        match.playerHealth = player.GetComponent<Health>();
        match.statusText = CreateStatusText();

        Selection.activeGameObject = player;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("Training scene created at " + ScenePath);
    }

    private static GameObject CreateBotPrefab()
    {
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (existing != null) return existing;

        GameObject bot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        bot.name = "Training Bot";
        bot.transform.position = new Vector3(0f, 1f, 0f);
        Object.DestroyImmediate(bot.GetComponent<CapsuleCollider>());
        CharacterController cc = bot.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.radius = 0.42f;
        cc.center = new Vector3(0f, 1f, 0f);
        bot.AddComponent<BRTargetHealth>();
        bot.AddComponent<TrainingBotPatrol>();
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(bot, PrefabPath);
        Object.DestroyImmediate(bot);
        return prefab;
    }

    private static void CreateArena()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Arena Ground";
        floor.transform.position = new Vector3(0f, -0.5f, 0f);
        floor.transform.localScale = new Vector3(120f, 1f, 120f);

        for (int i = 0; i < 12; i++)
        {
            float angle = i * Mathf.PI * 2f / 12f;
            GameObject cover = GameObject.CreatePrimitive(i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Cylinder);
            cover.name = "Cover " + (i + 1);
            cover.transform.position = new Vector3(Mathf.Cos(angle) * 12f, 0.8f, Mathf.Sin(angle) * 12f);
            cover.transform.localScale = i % 2 == 0 ? new Vector3(3f, 1.6f, 2f) : new Vector3(2f, 1.6f, 2f);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = "Training Building " + (i + 1);
            building.transform.position = new Vector3((i % 2 == 0 ? -1f : 1f) * 28f, 3f, (i < 2 ? -1f : 1f) * 24f);
            building.transform.localScale = new Vector3(12f, 6f, 10f);
        }
    }

    private static Transform[] CreateSpawnPoints()
    {
        Transform[] points = new Transform[6];
        for (int i = 0; i < points.Length; i++)
        {
            float angle = i * Mathf.PI * 2f / points.Length;
            GameObject point = new GameObject("Bot Spawn " + (i + 1));
            point.transform.position = new Vector3(Mathf.Cos(angle) * 18f, 1f, Mathf.Sin(angle) * 18f);
            points[i] = point.transform;
        }
        return points;
    }

    private static void CreatePickup(string name, PrimitiveType shape, Vector3 position, System.Type pickupType)
    {
        GameObject item = GameObject.CreatePrimitive(shape);
        item.name = name;
        item.transform.position = position;
        item.transform.localScale = Vector3.one * 0.8f;
        Collider collider = item.GetComponent<Collider>();
        collider.isTrigger = true;
        item.AddComponent<Rigidbody>().isKinematic = true;
        item.AddComponent(pickupType);
    }

    private static Text CreateStatusText()
    {
        GameObject canvasObject = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject textObject = new GameObject("Match Status", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(canvasObject.transform, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -28f);
        rect.sizeDelta = new Vector2(900f, 70f);
        Text label = textObject.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 34;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = "PRACTICE MATCH";
        return label;
    }

    private static void CreateLight()
    {
        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
        string folder = System.IO.Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, folder);
    }
}
#endif
