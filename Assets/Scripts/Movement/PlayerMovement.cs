using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed; 
    private Vector2 _moveDirection;
    public LayerMask ground;
    public Transform groundCheck;
    [SerializeField] bool grounded;

    

    public InputActionReference move;
    

    // Update is called once per frame
   private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(_moveDirection.x * moveSpeed, velocity.y, _moveDirection.y * moveSpeed);

    }
   
}
