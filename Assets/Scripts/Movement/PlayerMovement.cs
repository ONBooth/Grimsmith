using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody rb;
    public float moveSpeed;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public LayerMask ground;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    [SerializeField] private bool grounded;

    private Vector2 _moveDirection;

    public void SetMoveDirection(Vector2 direction)
    {
        _moveDirection = direction;
    }

    private void FixedUpdate()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, ground);

        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(_moveDirection.x * moveSpeed, velocity.y, _moveDirection.y * moveSpeed);
    }

    public void Jump()
    {
        if (!grounded) return;

        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z); // reset vertical velocity for consistent jump height
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}