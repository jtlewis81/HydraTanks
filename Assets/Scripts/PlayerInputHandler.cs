using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 
///     Essentially a translation layer between the PlayerInputActions (Input System) and the player's TankController.
/// 
/// </summary>

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // camera reference used to calculate Mouse position
    [SerializeField] private float gamepadDeadzone = 0.1f; // gamepad stick deadzone override

    private PlayerInputActions playerInputActions; // the input actions asset that defines the controls (has: control scheme > action map > actions)

    public static bool IsGamepad { get; private set; } // the current controller scheme the player is using is a gamepad

    // unity events that the tank controller methods get assigned to in the editor
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnAimTurret = new UnityEvent<Vector2>();

    private void Awake()
    {
        // get references
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        playerInputActions = new PlayerInputActions();
    }

    // InputActions must be enabled/disabled
    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    // get inputs every frame
    private void Update()
    {
        GetBodyMovement();
        GetTurretRotation();
        GetShootInput();
        CheckPause();
    }

    /// <summary>
    /// 
    ///     Gets movement inputs from the player's input hardware and uses a Unity Event to trigger the assigned method in the TankController
    ///     Keyboard/Mouse: WASD keys
    ///     Gamepad:        Left stick
    /// 
    /// </summary>
    private void GetBodyMovement()
    {
        Vector2 movementVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        OnMoveBody?.Invoke(movementVector.normalized);
    }

    /// <summary>
    /// 
    ///     Gets aiming input from the player's input hardware and uses a Unity Event to trigger the assigned method in the TankController
    ///     Keyboard/Mouse: Mouse movement vector
    ///     Gamepad:        Right Stick
    /// 
    /// </summary>
    private void GetTurretRotation()
    {
        if (IsGamepad)
        {
            if(playerInputActions.Player.Aim.ReadValue<Vector2>().magnitude > gamepadDeadzone)
            {
                OnAimTurret?.Invoke(playerInputActions.Player.Aim.ReadValue<Vector2>().normalized);
            }
        }
        else
        {
            OnAimTurret?.Invoke(GetMouseWorldPosition());
        }
    }

    /// <summary>
    /// 
    ///     Helper method to calculate turret rotation.
    ///     Gets the mouse position.
    /// 
    /// </summary>
    /// <returns></returns>
    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePos);
        return mouseWorldPosition;
    }

    /// <summary>
    /// 
    ///     Gets firing input from the player's input hardware and uses a Unity Event to trigger the assigned method in the TankController
    ///     Keyboard/Mouse: Left click on the Mouse
    ///     Gamepad:        Right trigger
    /// 
    /// </summary>
    private void GetShootInput()
    {
        if (!MenuSystem.Instance.IsPaused && playerInputActions.Player.Fire.WasPressedThisFrame())
        {
            OnShoot?.Invoke();
        }
    }

    /// <summary>
    /// 
    ///     Gets pause input from the player's input hardware and uses a Unity Event to trigger the assigned method in the TankController
    ///     Keyboard/Mouse: Escape Key
    ///     Gamepad:        Start button
    /// 
    /// </summary>
    private void CheckPause()
    {
        if (!MenuSystem.Instance.IsPaused && playerInputActions.Player.Pause.WasPressedThisFrame())
        {
            MenuSystem.Instance.PauseGame();
        }
    }

    /// <summary>
    /// 
    ///     Helper method assigned to the OnDeviceChanged Unity Event on the PlayerInput component.
    ///     Used to determine when the player is using a gamepad controller so that other helper methods work correctly.
    /// 
    /// </summary>
    /// <param name="input"></param>
    public void OnDeviceChange(PlayerInput input)
    {
        IsGamepad = input.currentControlScheme.Equals("Gamepad");
    }
}
