using UnityEngine;

public class EnemyController : MonoBehaviour, IGridOccupant, IDemageable, IAttacker
{
	private EnemyData _data;
	private GridSystem _gridSystem;
	private EnemyEventManager _eventManager;
	private float _currentHealth;
	
	public Vector2Int GridPosition { get; private set; }
	public Vector2Int FacingDirection { get; private set; }
	public float AttackPower => _data.AttackPower;
	public bool IsAttacking { get; private set; }
	public float MaxHealth => _data.MaxHealth;

	private void Awake()
	{
		_eventManager = GetComponent<EnemyEventManager>();
	}

	public void Initialize(EnemyData data, GridSystem gridSystem, Vector2Int spawnPoint)
    {

        _data = data;
        _currentHealth = data.MaxHealth;
        _gridSystem = gridSystem;
        GridPosition = spawnPoint;
        transform.position = _gridSystem.GridToWorld(spawnPoint);
        _gridSystem.Register(spawnPoint, this);
		
    }

	public void TakeDamage(float amount)
	{
		_currentHealth = Mathf.Max(_currentHealth - amount, 0);
		Debug.Log($"[Enemy] {_data.Name} took {amount} damage. HP: {_currentHealth}/{MaxHealth}");
		_eventManager.NotifyObservers(new EnemyEvent
		{
			Event = EnemyEventType.TookDamage,
			Value = _currentHealth,
			MaxValue = MaxHealth
		});

		if (_currentHealth <= 0)
			Die();
	}

	private void Die()
	{
		Debug.Log($"[Enemy] {_data.Name} died at {GridPosition}.");
		_eventManager.NotifyObservers(new EnemyEvent { Event = EnemyEventType.Died });
		_gridSystem.Unregister(GridPosition);
		Destroy(gameObject);
	}

	public void SetAttacking(bool value) => IsAttacking = value;

	public void NotifyAttack()
	{
		_eventManager.NotifyObservers(new EnemyEvent
		{
			Event = EnemyEventType.Attacking
		});
	}

    public void RegisterObserver(IObserver<EnemyEvent> observer)
    {
        _eventManager.RegisterObserver(observer);
    }

    public void UnregisterObserver(IObserver<EnemyEvent> observer)
    {
        _eventManager.UnregisterObserver(observer);
    }
}
