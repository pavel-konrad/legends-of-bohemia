using System.Collections;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    protected PlayerController _player;
    protected GridSystem _grid;
    protected PlayerData _data;

    public void Initialize(GridSystem grid, PlayerData data)
    {
        _player = GetComponent<PlayerController>();
        _grid = grid;
        _data = data;
    }

    public virtual void OnHeal(float amount, float duration)
    {
        _player.HealEffect(amount * _data.HealMultiplier, duration);
        if (_data.HealSpreadAmount > 0f)
            SpreadHeal(amount * _data.HealSpreadAmount, duration);
    }

    public virtual void OnPoison(float damage, float duration)
    {
        _player.PoisonEffect(damage * _data.PoisonMultiplier, duration);
        if (_data.PoisonSpreadAmount > 0f)
            SpreadPoison(damage * _data.PoisonSpreadAmount, duration);
    }

    public virtual void OnModifySpeed(float amount, float duration)
        => _player.SpeedEffect(amount * _data.SpeedMultiplier, duration);

    protected void SpreadHeal(float amount, float duration)
    {
        foreach (Vector2Int cell in GetAdjacentCells())
        {
            IGridOccupant occupant = _grid.GetOccupant(cell);
            if (occupant is ISpellTarget target)
                target.Heal(amount, duration);
        }
    }

    protected void DrainFromEnemies(float amount)
    {
        foreach (Vector2Int cell in GetAdjacentCells())
        {
            IGridOccupant occupant = _grid.GetOccupant(cell);
            if (occupant is IDemageable enemy)
                enemy.TakeDamage(amount);
        }
    }

    protected void SpreadPoison(float damage, float duration)
        => StartCoroutine(SpreadPoisonRoutine(damage, duration));

    private IEnumerator SpreadPoisonRoutine(float damage, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float delta = damage * (Time.deltaTime / duration);
            foreach (Vector2Int cell in GetAdjacentCells())
            {
                IGridOccupant occupant = _grid.GetOccupant(cell);
                if (occupant is IDemageable damageable)
                    damageable.TakeDamage(delta);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    protected Vector2Int[] GetAdjacentCells()
    {
        Vector2Int pos = _player.GridPosition;
        return new[]
        {
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
            pos + Vector2Int.right,
            pos + new Vector2Int( 1,  1),
            pos + new Vector2Int(-1,  1),
            pos + new Vector2Int( 1, -1),
            pos + new Vector2Int(-1, -1),
        };
    }
}
