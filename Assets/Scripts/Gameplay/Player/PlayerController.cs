using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IGridOccupant, ISpellTarget, IMovable, IDemageable, IAttacker
{
    [SerializeField] private InputActionAsset _inputActions;
    private PlayerEventManager _playerEventManager;
    private InputAction _moveAction;

    private AttackBase _attackController;
    private CharacterBehavior _characterBehavior;


    private bool _isMoving;
    private bool _wasMoving;
    private PlayerData _data;

    public float AttackPower => _data.AttackPower;
    public bool IsAttacking { get; private set; }
    public void SetAttacking(bool value) => IsAttacking = value;

    public Vector2Int FacingDirection {get; private set; }

    public float MaxHealth => _data.MaxHealth;
    public float MaxEnergy => _data.MaxEnergy;
    public float MoveSpeed => _data.MoveSpeed;
    public float CurrentHealth { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float CurrentSpeed { get; private set; }
    
    private GridSystem _gridSystem;
    private SpellController _spellController;
    public Vector2Int GridPosition {get; private set;}
    

    private void Awake()
    {
        _moveAction = _inputActions.FindActionMap("Player").FindAction("Move");
        _attackController = GetComponent<AttackBase>();
        _spellController = GetComponent<SpellController>();
        _playerEventManager = GetComponent<PlayerEventManager>();

    }
    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
    void Update()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            if (_wasMoving && !_isMoving)
            {
                _wasMoving = false;
                _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.Standing,
                Value = 0f,
                MaxValue = MoveSpeed
            });
            }return;
        } 

        Vector2Int direction = new Vector2Int(
            Mathf.RoundToInt(input.x),
            Mathf.RoundToInt(input.y)
       );
        _wasMoving = true;
        Move(direction);
    }

    public void Move(Vector2Int direction)
    {
        Vector2Int targetCell = GridPosition + direction;

        if (!_gridSystem.IsValid(targetCell)) return;

        FacingDirection = direction;

        if (_isMoving) return;
        if (IsAttacking) return;

        if (!_gridSystem.IsCellFree(targetCell))
        {
            StartCoroutine(TurnRoutine(direction));
            IGridOccupant occupant = _gridSystem.GetOccupant(targetCell);
            if (occupant is ICollectable collectable)
                CollectSpell(occupant, targetCell);
            return;
        }

        StartCoroutine(MoveRoutine(targetCell));
    }
    private IEnumerator TurnRoutine(Vector2Int direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsed);
            elapsed += Time.deltaTime * CurrentSpeed;
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    private IEnumerator MoveRoutine(Vector2Int targetCell)
    {
        
        _isMoving = true;
        if (CurrentSpeed > MoveSpeed)
        {
            _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.Running,
                Value = CurrentSpeed,
                MaxValue = MoveSpeed
            });
        }
        else
        {
            _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.Walking,
                Value = CurrentSpeed,
                MaxValue = MoveSpeed
            });
        }
        

        Vector2Int direction = targetCell - GridPosition;
        FacingDirection = direction;

        Quaternion targetRotation = Quaternion.LookRotation(
            new Vector3(direction.x, 0, direction.y)
            );

        Vector3 startPos = transform.position;
        Vector3 endPos = _gridSystem.GridToWorld(targetCell);
        
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsed);
            elapsed += Time.deltaTime * CurrentSpeed;
            yield return null;
        }
        
        transform.position = endPos;

        bool moved = _gridSystem.Move(GridPosition, targetCell, this);
        if (moved)
            GridPosition = targetCell;
        

        _isMoving = false;
        
        
    }

    public void CollectSpell(IGridOccupant occupant, Vector2Int targetCell)
    {
        ICollectable collectable = occupant as ICollectable;
        _gridSystem.Unregister(targetCell);
        collectable.OnCollected(this);
        ISpell spell = occupant as ISpell;
        _spellController.Enqueue(spell);
        Destroy((occupant as MonoBehaviour).gameObject);
    }

    public void Heal(float amount, float duration)
        => _characterBehavior.OnHeal(amount, duration);

    public void HealEffect(float amount, float duration)
        => StartCoroutine(HealRoutine(amount, duration));

    IEnumerator HealRoutine(float amount, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float delta = amount * (Time.deltaTime / duration);
            CurrentHealth = Mathf.Min(CurrentHealth + delta, MaxHealth);
            _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.HealthChanged,
                Value = CurrentHealth,
                MaxValue = MaxHealth
            });
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void ModifySpeed(float amount, float duration)
        => _characterBehavior.OnModifySpeed(amount, duration);

    public void SpeedEffect(float amount, float duration)
        => StartCoroutine(SpeedRoutine(amount, duration));

    IEnumerator SpeedRoutine(float amount, float duration)
    {
        float elapsed = 0;
        CurrentSpeed = CurrentSpeed * amount;
        _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.Running,
                Value = CurrentSpeed,
                MaxValue = MoveSpeed
            });
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        CurrentSpeed = CurrentSpeed / amount;
         _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.Walking,
                Value = CurrentSpeed,
                MaxValue = MoveSpeed
            });
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        _playerEventManager.NotifyObservers(new PlayerEvent
        {
            Event = PlayerEventType.HealthChanged,
            Value = CurrentHealth,
            MaxValue = MaxHealth
        });
    }
    public void ApplyPoison(float damage, float duration)
        => _characterBehavior.OnPoison(damage, duration);

    public void PoisonEffect(float damage, float duration)
        => StartCoroutine(ApplyPoisonCorountine(damage, duration));
    IEnumerator ApplyPoisonCorountine(float damage, float duration)
    {
        float elapsed = 0;
        while(elapsed < duration)
        {
            float delta = damage * (Time.deltaTime / duration);
            CurrentHealth = Mathf.Max(CurrentHealth - delta, 0);
            _playerEventManager.NotifyObservers(new PlayerEvent
            {
                Event = PlayerEventType.HealthChanged,
                Value = CurrentHealth,
                MaxValue = MaxHealth
            });
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void OnAttack() => _attackController.ExecuteAttack();

    public void OnAttackFinished() => SetAttacking(false);

    public void NotifyAttack()                                                                                        
    {                                                                                                                 
        _playerEventManager.NotifyObservers(new PlayerEvent                                                           
        {                                                                                                             
            Event = PlayerEventType.Attacking                 
        });                                                                                                           
    }   

    public void Initialize(PlayerData data, GridSystem gridSystem, Vector2Int spawnPoint)
    {

        _data = data;
        CurrentHealth = data.MaxHealth;
        CurrentEnergy = data.MaxEnergy;
        CurrentSpeed = data.MoveSpeed;
        _gridSystem = gridSystem;
        GridPosition = spawnPoint;
        transform.position = _gridSystem.GridToWorld(spawnPoint);
        _gridSystem.Register(spawnPoint, this);
        _characterBehavior = GetComponent<CharacterBehavior>();

        _attackController.Initialize(
            _inputActions.FindActionMap("Player").FindAction("Attack"),
            gridSystem
        );
    }

    public void RegisterObserver(IObserver<PlayerEvent> observer)
    {
        _playerEventManager.RegisterObserver(observer);
    }
    public void UnregisterObserver(IObserver<PlayerEvent> observer)
    {
        _playerEventManager.UnregisterObserver(observer);
    }

}
