using UnityEngine;

// Builds a simple greybox practice arena from Unity primitives at runtime.
public class ProceduralTrainingArena : MonoBehaviour
{
    public int arenaSize = 120;
    public int coverCount = 24;
    public int buildingCount = 5;
    public int randomSeed = 77;
    public float groundThickness = 1f;
    public Transform playerSpawn;

    private void Start() { Generate(); }

    [ContextMenu("Generate Arena")]
    public void Generate()
    {
        Random.InitState(randomSeed);
        CreateBlock("Arena Ground", new Vector3(0f, -groundThickness * 0.5f, 0f), new Vector3(arenaSize, groundThickness, arenaSize), new Color(0.28f, 0.34f, 0.25f));
        float half = arenaSize * 0.5f;
        CreateBlock("North Boundary", new Vector3(0f, 2f, half), new Vector3(arenaSize, 4f, 1f), Color.gray);
        CreateBlock("South Boundary", new Vector3(0f, 2f, -half), new Vector3(arenaSize, 4f, 1f), Color.gray);
        CreateBlock("East Boundary", new Vector3(half, 2f, 0f), new Vector3(1f, 4f, arenaSize), Color.gray);
        CreateBlock("West Boundary", new Vector3(-half, 2f, 0f), new Vector3(1f, 4f, arenaSize), Color.gray);

        for (int i = 0; i < coverCount; i++)
        {
            Vector3 p = RandomArenaPoint(10f);
            float sx = Random.Range(2f, 5f);
            float sz = Random.Range(2f, 5f);
            float h = Random.Range(1.2f, 3f);
            CreateBlock("Cover " + (i + 1), new Vector3(p.x, h * 0.5f, p.z), new Vector3(sx, h, sz), new Color(0.38f, 0.39f, 0.42f));
        }

        for (int i = 0; i < buildingCount; i++)
        {
            Vector3 p = RandomArenaPoint(22f);
            CreateBuilding(p, i + 1);
        }

        if (playerSpawn != null) playerSpawn.position = new Vector3(0f, 1.2f, -half + 12f);
    }

    private Vector3 RandomArenaPoint(float keepClear)
    {
        Vector3 p;
        int attempts = 0;
        do
        {
            p = new Vector3(Random.Range(-arenaSize * 0.4f, arenaSize * 0.4f), 0f, Random.Range(-arenaSize * 0.4f, arenaSize * 0.4f));
            attempts++;
        } while (p.magnitude < keepClear && attempts < 30);
        return p;
    }

    private void CreateBuilding(Vector3 p, int index)
    {
        float width = Random.Range(9f, 14f);
        float depth = Random.Range(9f, 14f);
        float height = Random.Range(5f, 8f);
        Color wall = new Color(0.62f, 0.57f, 0.46f);
        CreateBlock("Training Building " + index + " Floor", p + new Vector3(0f, 0.15f, 0f), new Vector3(width, 0.3f, depth), new Color(0.35f, 0.34f, 0.31f));
        CreateBlock("Building " + index + " Back Wall", p + new Vector3(0f, height * 0.5f, depth * 0.5f), new Vector3(width, height, 0.35f), wall);
        CreateBlock("Building " + index + " Left Wall", p + new Vector3(-width * 0.5f, height * 0.5f, 0f), new Vector3(0.35f, height, depth), wall);
        CreateBlock("Building " + index + " Right Wall", p + new Vector3(width * 0.5f, height * 0.5f, 0f), new Vector3(0.35f, height, depth), wall);
        CreateBlock("Building " + index + " Front Left", p + new Vector3(-width * 0.28f, height * 0.5f, -depth * 0.5f), new Vector3(width * 0.44f, height, 0.35f), wall);
        CreateBlock("Building " + index + " Front Right", p + new Vector3(width * 0.28f, height * 0.5f, -depth * 0.5f), new Vector3(width * 0.44f, height, 0.35f), wall);
        CreateBlock("Building " + index + " Roof", p + new Vector3(0f, height, 0f), new Vector3(width + 0.5f, 0.4f, depth + 0.5f), new Color(0.25f, 0.28f, 0.31f));
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
