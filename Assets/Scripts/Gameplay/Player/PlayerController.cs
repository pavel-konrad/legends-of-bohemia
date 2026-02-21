
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;



public class PlayerController : MonoBehaviour, IGridOccupant, ISpellTarget
{
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction _moveAction;
    private bool _isMoving;
    private PlayerData _data;
    private GridSystem _gridSystem;
    public Vector2Int GridPosition {get;set;}
    

    private void Awake()
    {
        _moveAction = _inputActions.FindActionMap("Player").FindAction("Move");
    }
    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
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
        if (!_gridSystem.IsCellFree(targetCell)) return;

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
        _gridSystem.Move(GridPosition, targetCell, this);
        GridPosition = targetCell;

        _isMoving = false;
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
    public void ApplyPoision(float damage, float duration)
    {
        
    }
    public void Initialize(PlayerData data, GridSystem gridSystem, Vector2Int spawnPoint)
    {
        _data = data;
        _gridSystem = gridSystem;
        GridPosition = spawnPoint;
        transform.position = _gridSystem.GridToWorld(spawnPoint);
        _gridSystem.Register(spawnPoint, this);
    }
}
