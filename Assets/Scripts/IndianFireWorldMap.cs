using System.Collections.Generic;
using UnityEngine;

// Original fictional India-inspired battle-royale world for INDIAN FIRE.
// All structures and place names are procedural placeholders, not copied game assets.
public class IndianFireWorldMap : MonoBehaviour
{
    private const float MapWidth = 150f;
    private const float MapHeight = 180f;
    private const float TileSize = 7.5f;
    private static readonly Dictionary<Color, Material> Materials = new Dictionary<Color, Material>();

    private static readonly Vector2[] Outline =
    {
        new Vector2(-0.18f, 0.98f), new Vector2(0.12f, 0.94f),
        new Vector2(0.31f, 0.82f), new Vector2(0.43f, 0.66f),
        new Vector2(0.57f, 0.53f), new Vector2(0.62f, 0.34f),
        new Vector2(0.53f, 0.16f), new Vector2(0.43f, -0.02f),
        new Vector2(0.28f, -0.22f), new Vector2(0.15f, -0.49f),
        new Vector2(0.04f, -0.83f), new Vector2(-0.04f, -0.98f),
        new Vector2(-0.15f, -0.76f), new Vector2(-0.24f, -0.52f),
        new Vector2(-0.38f, -0.31f), new Vector2(-0.52f, -0.12f),
        new Vector2(-0.58f, 0.08f), new Vector2(-0.49f, 0.29f),
        new Vector2(-0.37f, 0.47f), new Vector2(-0.28f, 0.69f)
    };

    private struct Landmark
    {
        public string name;
        public Vector2 position;
        public Color color;
        public Landmark(string label, float x, float z, Color tint)
        {
            name = label;
            position = new Vector2(x, z);
            color = tint;
        }
    }

    private static readonly Landmark[] Landmarks =
    {
        new Landmark("SNOWCREST GATE", -0.02f, 0.78f, new Color(0.72f, 0.86f, 0.91f)),
        new Landmark("VICTORY ARCH", -0.03f, 0.54f, new Color(0.88f, 0.72f, 0.38f)),
        new Landmark("MOONSTONE PALACE", -0.34f, 0.23f, new Color(0.86f, 0.77f, 0.55f)),
        new Landmark("LIONGATE PORT", -0.43f, -0.12f, new Color(0.78f, 0.67f, 0.43f)),
        new Landmark("FOUR ARCH BAZAAR", 0.16f, 0.02f, new Color(0.83f, 0.61f, 0.38f)),
        new Landmark("RIVER CROWN", 0.34f, 0.48f, new Color(0.58f, 0.76f, 0.78f)),
        new Landmark("GOLDEN SANCTUARY", -0.26f, 0.72f, new Color(0.92f, 0.78f, 0.36f)),
        new Landmark("SUNSPIRE COAST", 0.49f, -0.12f, new Color(0.91f, 0.72f, 0.42f)),
        new Landmark("TECH VALLEY", 0.06f, -0.36f, new Color(0.42f, 0.75f, 0.69f)),
        new Landmark("LOTUS REACH", -0.05f, -0.62f, new Color(0.79f, 0.55f, 0.68f)),
        new Landmark("BACKWATER HAVEN", -0.19f, -0.79f, new Color(0.43f, 0.72f, 0.62f))
    };

    private void Start() { Generate(); }

    [ContextMenu("Generate INDIAN FIRE World Map")]
    public void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }

        CreateBlock("INDIAN FIRE Ocean", new Vector3(0f, -1.8f, 0f),
            new Vector3(230f, 1f, 250f), new Color(0.12f, 0.30f, 0.39f));

        int columns = Mathf.CeilToInt(MapWidth / TileSize);
        int rows = Mathf.CeilToInt(MapHeight / TileSize);
        for (int x = 0; x < columns; x++)
        for (int z = 0; z < rows; z++)
        {
            float px = -MapWidth * 0.5f + x * TileSize + TileSize * 0.5f;
            float pz = -MapHeight * 0.5f + z * TileSize + TileSize * 0.5f;
            Vector2 normalized = new Vector2(px / (MapWidth * 0.5f), pz / (MapHeight * 0.5f));
            if (!InsideOutline(normalized)) continue;

            Color terrain = normalized.y > 0.55f
                ? new Color(0.63f, 0.69f, 0.61f)
                : normalized.y < -0.48f
                    ? new Color(0.27f, 0.49f, 0.37f)
                    : normalized.x < -0.28f
                        ? new Color(0.60f, 0.48f, 0.31f)
                        : new Color(0.43f, 0.53f, 0.34f);

            CreateBlock("India Terrain Tile", new Vector3(px, -0.85f, pz),
                new Vector3(TileSize + 0.15f, 1f, TileSize + 0.15f), terrain);
        }

        CreateRoad(new Vector3(-24f, -0.28f, 48f), new Vector3(20f, -0.28f, 35f));
        CreateRoad(new Vector3(20f, -0.28f, 35f), new Vector3(-24f, -0.28f, 8f));
        CreateRoad(new Vector3(-24f, -0.28f, 8f), new Vector3(-32f, -0.28f, -11f));
        CreateRoad(new Vector3(-32f, -0.28f, -11f), new Vector3(12f, -0.28f, -32f));
        CreateRoad(new Vector3(12f, -0.28f, -32f), new Vector3(-4f, -0.28f, -56f));

        for (int i = 0; i < Landmarks.Length; i++)
            CreateLandmark(Landmarks[i]);
    }

    private bool InsideOutline(Vector2 point)
    {
        bool inside = false;
        for (int i = 0, j = Outline.Length - 1; i < Outline.Length; j = i++)
        {
            Vector2 a = Outline[i];
            Vector2 b = Outline[j];
            bool crosses = ((a.y > point.y) != (b.y > point.y)) &&
                (point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x);
            if (crosses) inside = !inside;
        }
        return inside;
    }

    private void CreateLandmark(Landmark landmark)
    {
        Vector3 p = new Vector3(landmark.position.x * MapWidth * 0.5f, 0f,
            landmark.position.y * MapHeight * 0.5f);

        CreateBlock(landmark.name + " Plaza", p + Vector3.up * 0.18f,
            new Vector3(19f, 0.35f, 19f), landmark.color);
        CreateBlock(landmark.name + " Landmark", p + Vector3.up * 2.2f,
            new Vector3(3.2f, 4f, 3.2f), new Color(0.78f, 0.76f, 0.68f));

        CreateBuilding(landmark.name + " - Market House", p + new Vector3(-13f, 0f, 7f),
            new Vector3(8f, 5f, 8f), new Color(0.69f, 0.54f, 0.39f));
        CreateBuilding(landmark.name + " - Storehouse", p + new Vector3(13f, 0f, 7f),
            new Vector3(9f, 6f, 8f), new Color(0.52f, 0.56f, 0.53f));
        CreateBuilding(landmark.name + " - Residence", p + new Vector3(0f, 0f, -14f),
            new Vector3(8f, 5f, 7f), new Color(0.72f, 0.66f, 0.52f));

        CreateLootAreaLabel(landmark.name + " LOOT AREA", p + new Vector3(0f, 6f, 11f));
        CreatePickup("Health Supply", PrimitiveType.Sphere, p + new Vector3(-4f, 0.9f, 2f),
            new Color(0.25f, 0.85f, 0.45f), true);
        CreatePickup("Ammo Supply", PrimitiveType.Cube, p + new Vector3(4f, 0.75f, 2f),
            new Color(0.95f, 0.72f, 0.2f), false);
    }

    private void CreateBuilding(string name, Vector3 center, Vector3 size, Color wallColor)
    {
        float wall = 0.45f;
        float halfX = size.x * 0.5f;
        float halfZ = size.z * 0.5f;
        float height = size.y;

        CreateBlock(name + " Back Wall", center + new Vector3(0f, height * 0.5f, halfZ),
            new Vector3(size.x, height, wall), wallColor);
        CreateBlock(name + " Left Wall", center + new Vector3(-halfX, height * 0.5f, 0f),
            new Vector3(wall, height, size.z), wallColor);
        CreateBlock(name + " Right Wall", center + new Vector3(halfX, height * 0.5f, 0f),
            new Vector3(wall, height, size.z), wallColor);
        CreateBlock(name + " Front Left", center + new Vector3(-size.x * 0.32f, height * 0.5f, -halfZ),
            new Vector3(size.x * 0.36f, height, wall), wallColor);
        CreateBlock(name + " Front Right", center + new Vector3(size.x * 0.32f, height * 0.5f, -halfZ),
            new Vector3(size.x * 0.36f, height, wall), wallColor);
        CreateBlock(name + " Roof", center + new Vector3(0f, height + 0.25f, 0f),
            new Vector3(size.x + 0.8f, 0.5f, size.z + 0.8f), new Color(0.32f, 0.29f, 0.27f));
        CreateBlock(name + " Doorstep", center + new Vector3(0f, 0.12f, -halfZ - 0.6f),
            new Vector3(2.2f, 0.24f, 1.2f), new Color(0.55f, 0.46f, 0.34f));
    }

    private void CreateRoad(Vector3 start, Vector3 end)
    {
        Vector3 delta = end - start;
        GameObject road = CreateBlock("Village Road", (start + end) * 0.5f,
            new Vector3(3.4f, 0.18f, delta.magnitude), new Color(0.25f, 0.25f, 0.23f));
        road.transform.rotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
    }

    private void CreateLootAreaLabel(string label, Vector3 position)
    {
        GameObject item = new GameObject(label);
        item.transform.SetParent(transform, true);
        item.transform.position = position;
        TextMesh text = item.AddComponent<TextMesh>();
        text.text = label;
        text.characterSize = 0.48f;
        text.fontSize = 36;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = new Color(1f, 0.9f, 0.55f);
    }

    private void CreatePickup(string label, PrimitiveType shape, Vector3 position, Color color, bool health)
    {
        GameObject item = GameObject.CreatePrimitive(shape);
        item.name = label;
        item.transform.SetParent(transform, true);
        item.transform.position = position;
        item.transform.localScale = Vector3.one * 0.8f;
        Collider collider = item.GetComponent<Collider>();
        collider.isTrigger = true;
        Rigidbody body = item.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        if (health) item.AddComponent<HealthPickup>();
        else item.AddComponent<AmmoPickup>();
        SetColor(item, color);
    }

    private GameObject CreateBlock(string objectName, Vector3 position, Vector3 scale, Color color)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = objectName;
        block.transform.SetParent(transform, true);
        block.transform.position = position;
        block.transform.localScale = scale;
        SetColor(block, color);
        return block;
    }

    private void SetColor(GameObject item, Color color)
    {
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer == null) return;
        Material material;
        if (!Materials.TryGetValue(color, out material))
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            material = new Material(shader);
            material.color = color;
            Materials.Add(color, material);
        }
        renderer.sharedMaterial = material;
    }
}
