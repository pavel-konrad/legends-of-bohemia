using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private SpellQueuePresenter _spellQueuePresenter;
    [SerializeField] private PlayerStatsPresenter _playerStatsPresenter; 

    private void Awake()
    {
        
        _spawnManager.OnPlayerSpawned += HandlePlayerSpawned;
    }
    private void HandlePlayerSpawned(PlayerController player)
    {
        SpellController spellController = player.GetComponent<SpellController>();
        _spellQueuePresenter.SetSpellController(spellController);
        _playerStatsPresenter.SetPlayer(player);
    }

    private void OnDestroy()
    {
        _spawnManager.OnPlayerSpawned -=  HandlePlayerSpawned;
    }
}