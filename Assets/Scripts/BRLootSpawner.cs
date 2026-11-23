using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assign original, properly licensed pickup prefabs and hand-placed spawn points.
public class BRLootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootEntry
    {
        public GameObject prefab;
        [Min(0f)] public float weight = 1f;
        [Min(0)] public int maxActive = 4;
    }

    public LootEntry[] lootTable;
    public Transform[] spawnPoints;
    public float respawnDelay = 25f;
    public bool spawnOnStart = true;

    private readonly Dictionary<Transform, GameObject> activeAtPoint = new Dictionary<Transform, GameObject>();

    private void Start()
    {
        if (!spawnOnStart || spawnPoints == null) return;
        foreach (Transform point in spawnPoints)
            if (point != null) SpawnAt(point);
    }

    public void SpawnAt(Transform point)
    {
        if (point == null || (activeAtPoint.ContainsKey(point) && activeAtPoint[point] != null)) return;
        GameObject prefab = ChoosePrefab();
        if (prefab == null) return;
        GameObject item = Instantiate(prefab, point.position, point.rotation, transform);
        activeAtPoint[point] = item;
        StartCoroutine(WatchPickup(point, item));
    }

    private IEnumerator WatchPickup(Transform point, GameObject item)
    {
        while (item != null) yield return null;
        if (respawnDelay > 0f) yield return new WaitForSeconds(respawnDelay);
        activeAtPoint.Remove(point);
        SpawnAt(point);
    }

    private GameObject ChoosePrefab()
    {
        if (lootTable == null || lootTable.Length == 0) return null;
        float total = 0f;
        foreach (LootEntry entry in lootTable)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0f || entry.maxActive <= 0) continue;
            int activeCount = CountActive(entry.prefab);
            if (activeCount < entry.maxActive) total += entry.weight;
        }
        if (total <= 0f) return null;

        float roll = Random.value * total;
        foreach (LootEntry entry in lootTable)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0f || entry.maxActive <= 0 ||
                CountActive(entry.prefab) >= entry.maxActive) continue;
            roll -= entry.weight;
            if (roll <= 0f) return entry.prefab;
        }
        return null;
    }

    private int CountActive(GameObject prefab)
    {
        int count = 0;
        foreach (KeyValuePair<Transform, GameObject> pair in activeAtPoint)
            if (pair.Value != null && pair.Value.name == prefab.name + "(Clone)") count++;
        return count;
    }
}
