using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private LayerMask triggerLayerMask;
    private const float PLAYER_RADIUS = .7f;
    private const float PLAYER_HEIGHT = 2f;

    public void SetGameInput(GameInput gameInput)
    {
        this.gameInput = gameInput;
    }
    private void Update()
    {
        HandleMovement();
        HandleTrigger();
    }

    private void HandleTrigger()
    {
        var overlapBox = Physics.OverlapBox(transform.position, transform.localScale * 2, Quaternion.identity,
            triggerLayerMask);

        if (overlapBox.Length > 0)
        {
            Debug.Log(overlapBox[0].gameObject.GetComponentInParent<MazeTile>()?.GetHexagon().MazePosition);
        }
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