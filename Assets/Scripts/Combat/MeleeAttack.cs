using UnityEngine;

public class MeleeAttack : AttackBase
{
    protected override void PrepareAttack()
    {
        Vector2Int targetCell = _attacker.GridPosition + _attacker.FacingDirection;
        IGridOccupant occupant = _grid.GetOccupant(targetCell);
        _pendingTarget = occupant is IDemageable damageable ? damageable : null;
    }
}
