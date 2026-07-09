using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed; 
    private Vector2 _moveDirection;

    public InputActionReference move;
    public InputActionReference jump ;

    // Update is called once per frame
   private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(x:_moveDirection.x * moveSpeed, y:_moveDirection.y * moveSpeed);
    }
}
