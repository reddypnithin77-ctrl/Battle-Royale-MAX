using System.Collections;
using UnityEngine;

public class BRTrainingBotSpawner : MonoBehaviour
{
    public GameObject botPrefab;
    public Transform[] spawnPoints;
    [Min(0)] public int botCount = 6;
    public float spawnInterval = 0.5f;
    public bool spawnOnStart = true;

    private IEnumerator Start()
    {
        if (!spawnOnStart || botPrefab == null || spawnPoints == null || spawnPoints.Length == 0) yield break;
        for (int i = 0; i < botCount; i++)
        {
            Transform point = spawnPoints[i % spawnPoints.Length];
            if (point != null) Instantiate(botPrefab, point.position, point.rotation);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
