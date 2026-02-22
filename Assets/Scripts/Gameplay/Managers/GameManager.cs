using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private SpellQueuePresenter _spellQueuePresenter;
    // [SerializeField] private PlayerStatsPresenter _playerStatsPresenter; 

    public void Awake()
    {
        _spawnManager.OnPlayerSpawned += _spellQueuePresenter.SetPlayer;
    }

    private void OnDestroy()
    {
        _spawnManager.OnPlayerSpawned -= _spellQueuePresenter.SetPlayer;
    }
}