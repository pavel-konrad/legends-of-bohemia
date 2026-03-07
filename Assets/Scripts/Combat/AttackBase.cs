using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AttackBase : MonoBehaviour
{
    protected IAttacker _attacker;
    protected GridSystem _grid;
    protected IDemageable _pendingTarget;
    private InputAction _attack;

    protected virtual void Awake()
    {
        _attacker = GetComponent<IAttacker>();
    }

    public void Initialize(InputAction attackAction, GridSystem grid)
    {
        _attack = attackAction;
        _grid = grid;
        _attack.Enable();
    }

    protected void OnDisable() => _attack?.Disable();

    protected virtual void Update()
    {
        if (_attack == null) return;
        if (_attacker.IsAttacking) return;

        if (_attack.WasPressedThisFrame())
        {
            if (_attacker.FacingDirection == Vector2Int.zero) return;
            PrepareAttack();
            _attacker.SetAttacking(true);
            _attacker.NotifyAttack();
        }
    }

    protected abstract void PrepareAttack();

    public virtual void ExecuteAttack()
    {
        _pendingTarget?.TakeDamage(_attacker.AttackPower);
        _pendingTarget = null;
    }
}
