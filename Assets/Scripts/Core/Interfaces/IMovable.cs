using UnityEngine;
public interface IMovable
{
    void Move(Vector2Int direction);
    float MoveSpeed { get; }

}