using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float gamepadDeadzone = 0.1f;

    private PlayerInputActions playerInputActions; // the input actions asset that defines the controls (has: control scheme > action map > actions)

    public static bool IsGamepad { get; private set; }

    // unity events that the tank controller methods get assigned to in the editor
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnAimTurret = new UnityEvent<Vector2>();

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    private void Update()
    {
        GetBodyMovement();
        GetTurretRotation();
        GetShootInput();
        CheckPause();
    }

    private void GetBodyMovement()
    {
        Vector2 movementVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        OnMoveBody?.Invoke(movementVector.normalized);
    }

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

    // turret rotation helper
    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePos);
        return mouseWorldPosition;
    }

    private void GetShootInput()
    {
        if (!MenuSystem.Instance.IsPaused && playerInputActions.Player.Fire.WasPressedThisFrame())
        {
            OnShoot?.Invoke();
        }
    }

    private void CheckPause()
    {
        if (!MenuSystem.Instance.IsPaused && playerInputActions.Player.Pause.WasPressedThisFrame())
        {
            MenuSystem.Instance.PauseGame();
        }
    }

    public void OnDeviceChange(PlayerInput input)
    {
        IsGamepad = input.currentControlScheme.Equals("Gamepad");
    }
}
