using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
{
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        // ...
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // jump
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // attack
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // crouch
        }
        else if (context.canceled)
        {
            // stand back up, if crouch is a hold
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // interact
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 look = context.ReadValue<Vector2>();
        // camera/mouse look
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // next (e.g. cycle weapon/item forward)
        }
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // previous (e.g. cycle weapon/item backward)
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // start sprinting
        }
        else if (context.canceled)
        {
            // stop sprinting
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // start dash
        }
        else if (context.canceled)
        {
            // stop dash
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // start pause
        }
        else if (context.canceled)
        {
            // stop pause
        }
    }
}