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
        
        Vector2Int randomCell = freeCells[Random.Range(0, freeCells.Count)];
        Vector3 worldPos = _gridSystem.GridToWorld(randomCell);
        
        GameObject prefab = _spellFactory.Create(SpellType.Heal);  // zatím natvrdo
        GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
        
        // zaregistruj do gridu
        IGridOccupant occupant = instance.GetComponent<IGridOccupant>();
        _gridSystem.Register(randomCell, occupant);
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