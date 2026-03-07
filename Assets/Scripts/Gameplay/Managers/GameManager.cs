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
        SpellEventManager spellManager = player.GetComponent<SpellEventManager>();
        _spellQueuePresenter.SetSpellEventManager(spellManager);
        player.GetComponent<SpellSoundController>().SetSpellEventManager(spellManager);
        _playerStatsPresenter.SetPlayer(player);
    }

    private void OnDestroy()
    {
        _spawnManager.OnPlayerSpawned -=  HandlePlayerSpawned;
    }
}