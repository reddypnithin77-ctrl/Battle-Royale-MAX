using UnityEngine;

// Original, stylized India-inspired world map for INDIAN FIRE.
// This is a fictional game layout, not a geographic or navigational map.
public class IndianFireWorldMap : MonoBehaviour
{
    private const float MapWidth = 150f;
    private const float MapHeight = 180f;
    private const float TileSize = 7.5f;

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
            Destroy(transform.GetChild(i).gameObject);

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

        GameObject plaza = CreateBlock(landmark.name, p + Vector3.up * 0.35f,
            new Vector3(10f, 0.7f, 10f), landmark.color);
        CreateBlock(landmark.name + " Landmark", p + Vector3.up * 2.2f,
            new Vector3(3.2f, 4f, 3.2f), new Color(0.78f, 0.76f, 0.68f));

        GameObject label = new GameObject(landmark.name + " Label");
        label.transform.SetParent(transform, true);
        label.transform.position = p + new Vector3(0f, 5.2f, 0f);
        TextMesh text = label.AddComponent<TextMesh>();
        text.text = landmark.name;
        text.characterSize = 0.65f;
        text.fontSize = 48;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = Color.white;
    }

    private GameObject CreateBlock(string objectName, Vector3 position, Vector3 scale, Color color)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = objectName;
        block.transform.SetParent(transform, true);
        block.transform.position = position;
        block.transform.localScale = scale;
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader != null) renderer.material = new Material(shader) { color = color };
        }
        return block;
    }
}
