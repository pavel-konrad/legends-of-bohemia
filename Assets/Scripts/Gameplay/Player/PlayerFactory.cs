using UnityEngine;
/*

*/
public class PlayerFactory : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GridSystem _gridSystem;

    public GameObject Create(PlayerData data, Vector2Int spawnPoint)
    {
        GameObject instance = Instantiate(_playerPrefab);
        instance.GetComponent<PlayerController>().Initialize(data, _gridSystem, spawnPoint);
        return instance;
    }
}