using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Grid/GridConfig")]
public class GridConfig : ScriptableObject
{
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public float CellSize { get; private set; }

}