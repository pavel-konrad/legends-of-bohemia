using UnityEngine;
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

    public void Start()
    {
        if (!_spellFactory || !_gridSystem || !_settings)
        {
            Debug.LogError("Data from factory, grid or settings is not assigned");
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    // public void Update()
    // {
       

    // }

    private void SpawnSpell()
    {
        List<Vector2Int> freeCells = _gridSystem.GetFreeCells();

        if (freeCells.Count == 0) return; 
        // if (freeCells.Count >= _settings.MaxActiveSpells) return;

        Vector2Int randomCell = freeCells[Random.Range(0, freeCells.Count)];
        Vector3 worldPos = _gridSystem.GridToWorld(randomCell);
    
        SpellType type = GetRandomSpellType();
        
        GameObject prefab = _spellFactory.Create(type); 
        GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);   

        IGridOccupant occupant = instance.GetComponent<IGridOccupant>();
        _gridSystem.Register(randomCell, occupant);
        
      
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
}