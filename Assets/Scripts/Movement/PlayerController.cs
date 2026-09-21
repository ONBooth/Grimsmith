using UnityEngine;
using UnityEngine.InputSystem;
using Game.UI;
using Games.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private InputActionAsset _inputActionAsset;
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _pauseAction;
    
    [SerializeField] private PauseMenu _pauseMenu;

    private void OnEnable()
    {
        _inputActionAsset.Enable();
    }

    private void OnDisable()
    {
        _inputActionAsset.Disable();
    }

    private void Awake()
    {
        _moveAction.action.performed += OnMovePerformed;
        _moveAction.action.canceled += OnMoveCanceled;

        _jumpAction.action.performed += OnJumpPerformed;
        _jumpAction.action.canceled += OnJumpCanceled;
    }

    private void OnDestroy()
    {
        _moveAction.action.performed -= OnMovePerformed;
        _moveAction.action.canceled -= OnMoveCanceled;

        _jumpAction.action.performed -= OnJumpPerformed;
        _jumpAction.action.canceled -= OnJumpCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _playerMovement.SetMoveDirection(ctx.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _playerMovement.SetMoveDirection(Vector2.zero);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _playerMovement.Jump();
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
       // no action needed yet \('*')/
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        
    }
    
}