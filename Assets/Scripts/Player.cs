using System;
using MazeGenerator;
using UnityEngine;

public class Player : MonoBehaviour
{
    public class NewHexEventArgs
    {
        public MazeHexagon Hexagon;
    }

    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private LayerMask triggerLayerMask;
    [SerializeField] private LayerMask goalLayerMask;
    private const float PLAYER_RADIUS = .7f;
    private const float PLAYER_HEIGHT = 2f;
    private Collider _currentCollider;
    public bool IsMovementAllowed { get; set; }

    public static Player Instance { get; private set; }

    public event EventHandler<NewHexEventArgs> OnMoveToNewHex;
    public event EventHandler OnGoalTouched;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of player!");
        }

        Instance = this;
        IsMovementAllowed = true;
    }

    private void Update()
    {
        if (IsMovementAllowed)
        {
            HandleMovement();
            HandleNewHexagonTrigger();
            HandleGoal();
        }
    }

    private void HandleNewHexagonTrigger()
    {
        var overlapBox = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * PLAYER_HEIGHT,
            PLAYER_RADIUS, triggerLayerMask);

        if (overlapBox.Length > 1)
            Debug.LogError($"Number of overlapped boxes {overlapBox.Length}");
        if (overlapBox.Length <= 0 || overlapBox[0] == _currentCollider) return;
        _currentCollider = overlapBox[0];
        OnMoveToNewHex?.Invoke(this,
            new NewHexEventArgs {Hexagon = overlapBox[0].gameObject.GetComponentInParent<MazeTile>()?.GetHexagon()});
    }

    private void HandleGoal()
    {
        var overlapBox = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * PLAYER_HEIGHT,
            PLAYER_RADIUS, goalLayerMask);

        if (overlapBox.Length <= 0) return;

        OnGoalTouched?.Invoke(this, EventArgs.Empty);
    }

    private void HandleMovement()
    {
        var inputVector = gameInput.GetMovementVectorNormalized();

        var moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        var moveDistance = moveSpeed * Time.deltaTime;

        var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PLAYER_HEIGHT,
            PLAYER_RADIUS, moveDir, moveDistance, collisionLayerMask);

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        var rotateSpeed = 5f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}