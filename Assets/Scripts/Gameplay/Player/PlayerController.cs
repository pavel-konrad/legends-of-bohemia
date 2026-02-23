using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;



public class PlayerController : MonoBehaviour, IGridOccupant, ISpellTarget, IMovable, IDemageable
{
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction _moveAction;
    private bool _isMoving;
    private PlayerData _data;
    public float MaxHealth => _data.MaxHealth;
    public float MaxEnergy => _data.MaxEnergy;
    public float MoveSpeed => _data.MoveSpeed;
    public float CurrentHealth { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float CurrentSpeed { get; private set; }
    
    private GridSystem _gridSystem;
    private SpellController _spellController;
    public Vector2Int GridPosition {get; private set;}
    public event Action<float, float>OnHealthChanged;
    public event Action<float, float> OnEnergyChanged;
    

    private void Awake()
    {
        _moveAction = _inputActions.FindActionMap("Player").FindAction("Move");
        _spellController = GetComponent<SpellController>();

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

        if (input == Vector2.zero) return;

        Vector2Int direction = new Vector2Int(
            Mathf.RoundToInt(input.x),
            Mathf.RoundToInt(input.y)
       );

        Move(direction);
    }

    public void Move(Vector2Int direction)
    {
        if(_isMoving) return;

        Vector2Int targetCell = GridPosition + direction;

        if (!_gridSystem.IsValid(targetCell)) return;
        if (!_gridSystem.IsCellFree(targetCell)) 
        {
            IGridOccupant occupant = _gridSystem.GetOccupant(targetCell);
            if (occupant is ICollectable collectable)
            {
               CollectSpell(occupant, targetCell); 
            }
            else
            {
                return;
            }
            
           
        }

        StartCoroutine(MoveRoutine(targetCell));
    }
    private IEnumerator MoveRoutine(Vector2Int targetCell)
    {
        _isMoving = true;

        Vector2Int direction = targetCell - GridPosition;

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
    {

        StartCoroutine(HealRoutine(amount, duration));

    }

    IEnumerator HealRoutine(float amount, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float delta = amount * (Time.deltaTime / duration);
            CurrentHealth = Mathf.Min(CurrentHealth + delta, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void ModifySpeed(float amount, float duration)
    {
        StartCoroutine(SpeedRoutine(amount, duration));
    }

    IEnumerator SpeedRoutine(float amount, float duration)
    {
        float elapsed = 0;
        CurrentSpeed = CurrentSpeed * amount;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        CurrentSpeed = CurrentSpeed / amount;
    }

    public void TakeDamage(float amount)
    {
        
    }
    public void ApplyPoison(float damage, float duration)
    {
        StartCoroutine(ApplyPoisonCorountine(damage, duration));
    }
    IEnumerator ApplyPoisonCorountine(float damage, float duration)
    {
        float elapsed = 0;
        while(elapsed < duration)
        {
            float delta = damage * (Time.deltaTime / duration);
            CurrentHealth = Mathf.Max(CurrentHealth - delta, 0);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            elapsed += Time.deltaTime;
            yield return null;
        }
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

    }
}
