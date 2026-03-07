using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/*
This manager is responsible for spawning items, players and enemy on the World Grid. It has to be attached
on empty objects in the scene and add references to game objects with SpellFactory.cs, GridSystem.cs and
to the data from Game Settings scriptable object.
*/
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpellFactory _spellFactory;
    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private GameSettingsConfig _settings;
    [SerializeField] private PlayerFactory _playerFactory;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private EnemyFactory _enemyFactory;
    [SerializeField] private EnemySpawnConfig _enemySpawnConfig;
    private int _activeSpellCount;
    private int _activeEnemyCount;
    public event Action<PlayerController> OnPlayerSpawned;

    private void Awake()
    {
        _gridSystem.OnRegistered   += HandleOccupantRegistered;
        _gridSystem.OnUnregistered += HandleOccupantUnregistered;
    }
    private void OnDestroy()
    {
        _gridSystem.OnRegistered   -= HandleOccupantRegistered;
        _gridSystem.OnUnregistered -= HandleOccupantUnregistered;
    }
    private void Start()
    {
  
        
        if (!_spellFactory || !_gridSystem || !_settings)
        {
            #if UNITY_EDITOR
            Debug.LogError("Data from factory, grid or settings is not assigned");
            #endif
            return;
        }
        Initialize();

        
    }

    private void Initialize()
    {
        SpawnPlayer();
        StartCoroutine(SpawnRoutine());
        if (_enemyFactory != null && _enemySpawnConfig != null)
            StartCoroutine(EnemySpawnRoutine());
    }

    private void HandleOccupantRegistered(Vector2Int pos, IGridOccupant occupant)
    {
        if (occupant is ISpell) _activeSpellCount++;
        if (occupant is EnemyController) _activeEnemyCount++;
    }

    private void HandleOccupantUnregistered(Vector2Int pos, IGridOccupant occupant)
    {
        if (occupant is ISpell) _activeSpellCount--;
        if (occupant is EnemyController) _activeEnemyCount--;
    }
    public void SpawnPlayer()
    {
        if (_playerConfig.SelectedPlayer == null)
        {
            #if UNITY_EDITOR
            Debug.LogError("No player selected. Return to character select.");
            #endif
            return;
        }
        Vector2Int[] spawnPoints = GetPlayerSpawnPoints();
        GameObject playerObj = _playerFactory.Create(_playerConfig.SelectedPlayer, spawnPoints[0]);
        PlayerController player = playerObj.GetComponent<PlayerController>();
        OnPlayerSpawned?.Invoke(player);
    }
    private void SpawnEnemy()
    {
        if (_enemyFactory == null || _enemySpawnConfig == null) return;
        if (_activeEnemyCount >= _enemySpawnConfig.MaxActiveEnemies) return;

        List<Vector2Int> freeCells = _gridSystem.GetFreeCells();
        if (freeCells.Count == 0) return;

        Vector2Int randomCell = freeCells[UnityEngine.Random.Range(0, freeCells.Count)];
        EnemyData data = GetRandomEnemyData();
        if (data == null) return;

        _enemyFactory.Create(data, randomCell);
    }

    private EnemyData GetRandomEnemyData()
    {
        if (_enemySpawnConfig.EnemyWeights == null || _enemySpawnConfig.EnemyWeights.Count == 0) return null;

        int totalWeight = 0;
        foreach (var entry in _enemySpawnConfig.EnemyWeights)
            totalWeight += entry.Weight;

        int random = UnityEngine.Random.Range(0, totalWeight);
        foreach (var entry in _enemySpawnConfig.EnemyWeights)
        {
            random -= entry.Weight;
            if (random <= 0) return entry.Data;
        }
        return _enemySpawnConfig.EnemyWeights[0].Data;
    }

    public Vector2Int[] GetPlayerSpawnPoints()
    {
        Vector2Int center = _gridSystem.GetCenter();

        return new Vector2Int[]
        {
            new Vector2Int(center.x -1, center.y),
            new Vector2Int(center.x + 1, center.y),
            new Vector2Int(center.x, center.y - 1),
            new Vector2Int(center.x, center.y + 1),
        };
    }

    private void SpawnSpell()
    {
        
        
        if (_activeSpellCount >= _settings.MaxActiveSpells) return;

        List<Vector2Int> freeCells = _gridSystem.GetFreeCells();
        if (freeCells.Count == 0) return; 

        Vector2Int randomCell = freeCells[UnityEngine.Random.Range(0, freeCells.Count)];
        Vector3 worldPos = _gridSystem.GridToWorld(randomCell);
    
        SpellType type = GetRandomSpellType();
        
        GameObject instance = _spellFactory.Create(type);
        if (instance == null) return; 
        instance.transform.position = worldPos;
        
        SpellBase spell = instance.GetComponent<SpellBase>();
        if (spell == null) return;

        // spell.OnSpellCollected += HandleSpellCollected;  
        

        IGridOccupant occupant = instance.GetComponent<IGridOccupant>();

        bool registered = _gridSystem.Register(randomCell, occupant);
        if (!registered)
        {
            Destroy(instance);
            return;
        }
      
    }

    private SpellType GetRandomSpellType()
    {
        int totalWeight = 0;
        foreach (var entry in _settings.SpellWeights)
        {
            totalWeight += entry.Weight;
        }
        int random = UnityEngine.Random.Range(0, totalWeight);
        foreach (var entry in _settings.SpellWeights)
        {
            random -= entry.Weight;
            if (random <= 0) return entry.Type;
        }
        return _settings.SpellWeights[0].Type;

    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_settings.SpawnInterval);
            SpawnSpell();
        }
    }

    private IEnumerator EnemySpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_enemySpawnConfig.SpawnInterval);
            SpawnEnemy();
        }
    }
}