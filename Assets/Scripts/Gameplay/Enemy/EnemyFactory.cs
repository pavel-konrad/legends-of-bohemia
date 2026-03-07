using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GridSystem _gridSystem;

    public GameObject Create(EnemyData data, Vector2Int spawnPoint)
    {
        GameObject instance = Instantiate(data.EnemyPrefab);
        instance.GetComponent<EnemyController>().Initialize(data, _gridSystem, spawnPoint);
        instance.GetComponent<EnemySoundController>().Initialize(data);
        return instance;
    }
}
