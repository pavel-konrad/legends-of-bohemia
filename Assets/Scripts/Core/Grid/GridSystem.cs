using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
/*
This is a Grid system base on x and y coordinates. Data with configuration of grid is in scriptable objects
that is need to assign in inspector. Uses matrix math for coordinate conversion 
between Grid Space and World Space.
For Debuging is there a gyzmos and pointer, that showing this grid and clicked position in Scene View.
The Dictionary tracks positions of spawned objects like players, enemies and buffs.
Register() and Unregister() function adding coordinates with key and invokes an event, when is tile
occupied or free. For movement use Move() method in controllers.
*/
public class GridSystem : MonoBehaviour
{
    [SerializeField] private GridConfig config;
    private Dictionary<Vector2Int, IGridOccupant> _occupants;
    public event Action<Vector2Int, IGridOccupant> OnRegistered;
    public event Action<Vector2Int, IGridOccupant> OnUnregistered;

    private void Awake()
    {
        _occupants = new Dictionary<Vector2Int, IGridOccupant>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2Int gridPos = WorldToGrid(hit.point);
            }
        }
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * config.cellSize, 0, gridPos.y * config.cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
    return new Vector2Int(
        Mathf.RoundToInt(worldPos.x / config.cellSize),
        Mathf.RoundToInt(worldPos.z / config.cellSize)
        );
    }
    private void OnDrawGizmos()
    {
        if (config == null) return;
        if (_occupants == null) return; 
        
        Gizmos.color = Color.white;
        
        for (int x = 0; x < config.width; x++)
        {
            for (int y = 0; y < config.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 center = GridToWorld(new Vector2Int(x, y));
                if(!IsCellFree(pos))
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawWireCube(center, new Vector3(config.cellSize, 0, config.cellSize));
            }
        }
    }

    public bool IsValid(Vector2Int gridPos)
        {
            return
            gridPos.x >= 0 && gridPos.x < config.width &&
            gridPos.y >= 0 && gridPos.y < config.height;
        }

    public bool IsCellFree(Vector2Int gridPos)
    {
        return !_occupants.ContainsKey(gridPos);
    }

    public IGridOccupant GetOccupant(Vector2Int gridPos)
    {
        if (!_occupants.ContainsKey(gridPos)) return null;
        return _occupants[gridPos];
    }

    public void Register(Vector2Int gridPos, IGridOccupant occupant)
    {
        if (!IsValid(gridPos))
        {
            #if UNITY_EDITOR
            Debug.LogWarning("Position is not in grid");
            #endif
            return;
        }
        if (!IsCellFree(gridPos))
        {
            #if UNITY_EDITOR
            Debug.Log("Cell is occupied");
            #endif
            return;
        }
        _occupants.Add(gridPos, occupant);
        OnRegistered?.Invoke(gridPos, occupant);   
    }

    public void Unregister(Vector2Int gridPos)
    {
        if (!_occupants.ContainsKey(gridPos))
        {
            #if UNITY_EDITOR
            Debug.Log("There is nothing.");
            #endif
            return;
        }
        IGridOccupant occupant = _occupants[gridPos];
        _occupants.Remove(gridPos);
        OnUnregistered?.Invoke(gridPos, occupant);
    }
    
    //Unregisteres from old position and register to new one.
    public void Move(Vector2Int from, Vector2Int to, IGridOccupant occupant)
    {
        if (!IsValid(to))
        {
            #if UNITY_EDITOR
            Debug.LogWarning("Target possition is not in grid");
            #endif
            return;
        }
        if (!IsCellFree(to))
        {
            #if UNITY_EDITOR
            Debug.LogWarning("Targeted cell is occupied");
            #endif
            return;
        }

        Unregister(from);
        Register(to, occupant);
        return;
    }

    public List<Vector2Int> GetFreeCells()
    {
        List<Vector2Int> freeCells = new List<Vector2Int>();
        for (int x = 0; x < config.width; x++)
        {
            for (int y = 0; y < config.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (IsCellFree(pos))
                    freeCells.Add(pos);
            }
        }
        return freeCells;
    }
    public Vector2Int GetCenter()
    {
        return new Vector2Int(config.width / 2, config.height / 2);
    }
}