using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Grid/GridConfig")]
public class GridConfig : ScriptableObject
{
    public int width;
    public int height;
    public float cellSize;
}