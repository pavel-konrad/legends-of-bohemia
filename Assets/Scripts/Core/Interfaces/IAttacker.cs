using UnityEngine;

public interface IAttacker : IGridOccupant
{
	Vector2Int FacingDirection { get; }
	float AttackPower { get; }
	bool IsAttacking { get; }
	void NotifyAttack();
	void SetAttacking(bool value);
}
