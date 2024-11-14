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


    public event EventHandler<NewHexEventArgs> OnMoveToNewHex;
    public event EventHandler OnGoalTouched;

    private void Update()
    {
        HandleMovement();
        HandleNewHexagonTrigger();
        HandleGoal();
    }

    private void HandleNewHexagonTrigger()
    {
        var overlapBox = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity,
            triggerLayerMask);
        
        if (overlapBox.Length > 1)
            Debug.LogError($"Number of overlapped boxes {overlapBox.Length}");
        if (overlapBox.Length <= 0 || overlapBox[0] == _currentCollider) return;
        _currentCollider = overlapBox[0];
        OnMoveToNewHex?.Invoke(this,
            new NewHexEventArgs {Hexagon = overlapBox[0].gameObject.GetComponentInParent<MazeTile>()?.GetHexagon()});
    }

    private void HandleGoal()
    {
        var overlapBox = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity,
            goalLayerMask);

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