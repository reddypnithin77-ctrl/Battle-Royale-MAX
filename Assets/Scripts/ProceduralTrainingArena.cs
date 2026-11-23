using UnityEngine;

// Backward-compatible component: the old square training arena is retired.
// Existing scenes that still contain this component now generate the INDIAN FIRE world map.
[AddComponentMenu("INDIAN FIRE/World Map (Legacy Component Name)")]
public class ProceduralTrainingArena : MonoBehaviour
{
    public int randomSeed = 77;

    private void Start()
    {
        IndianFireWorldMap map = GetComponent<IndianFireWorldMap>();
        if (map == null)
        {
            // A newly added component generates itself in Start; avoid building twice.
            gameObject.AddComponent<IndianFireWorldMap>();
            return;
        }
        map.Generate();
    }

    [ContextMenu("Generate INDIAN FIRE World Map")]
    public void Generate()
    {
        IndianFireWorldMap map = GetComponent<IndianFireWorldMap>();
        if (map == null) map = gameObject.AddComponent<IndianFireWorldMap>();
        map.Generate();
    }
}
