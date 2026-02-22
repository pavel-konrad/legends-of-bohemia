using UnityEngine;


#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GridSystem))]
public class GridDebugger : UnityEngine.MonoBehaviour
{
    private GridSystem _grid;

    private void Awake()
    {
        _grid = GetComponent<GridSystem>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (UnityEngine.Physics.Raycast(ray, out UnityEngine.RaycastHit hit))
            {
                UnityEngine.Vector2Int gridPos = _grid.WorldToGrid(hit.point);
                UnityEngine.Debug.Log($"Grid position: {gridPos}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_grid == null) return;

        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                UnityEngine.Vector2Int pos = new UnityEngine.Vector2Int(x, y);
                UnityEngine.Vector3 center = _grid.GridToWorld(pos);
                UnityEngine.Gizmos.color = _grid.IsCellFree(pos)
                    ? UnityEngine.Color.white
                    : UnityEngine.Color.red;
                UnityEngine.Gizmos.DrawWireCube(center, new UnityEngine.Vector3(_grid.CellSize, 0, _grid.CellSize));
            }
        }
    }
}
#endif
