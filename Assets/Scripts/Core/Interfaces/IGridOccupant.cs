using UnityEngine;

//Interface for tracking free cells.

public interface IGridOccupant
{
    Vector2Int GridPosition {get; set;}
}
