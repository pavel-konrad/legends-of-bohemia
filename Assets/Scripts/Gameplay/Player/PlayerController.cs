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
    public float CurrentHealth { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float MoveSpeed => _data.MoveSpeed;
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
            elapsed += Time.deltaTime * _data.MoveSpeed;
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
        
    }
    public void ModifySpeed(float amount, float duration)
    {
        
    }

    public void TakeDamage(float amount)
    {
        
    }
    public void ApplyPoison(float damage, float duration)
    {
        
    }
    public void Initialize(PlayerData data, GridSystem gridSystem, Vector2Int spawnPoint)
    {

        _data = data;
        CurrentHealth = data.MaxHealth;
        CurrentEnergy = data.MaxEnergy;
        _gridSystem = gridSystem;
        GridPosition = spawnPoint;
        transform.position = _gridSystem.GridToWorld(spawnPoint);
        _gridSystem.Register(spawnPoint, this);

    }
}
