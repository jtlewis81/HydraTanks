using UnityEngine;
using UnityEngine.Events;

public class TankController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform turretBase;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turretSpeed;
    [SerializeField] private float reloadTimer;

    [Header("Editor Info [ DO NOT CHANGE ]")]
    [SerializeField] private float currentDelay;

    [Header("Events")]
    public UnityEvent OnShoot, OnCanShoot, OnCantShoot;
    public UnityEvent<float> OnReloading;

    private Collider2D[] tankColliders;
    private Vector2 movementVector;
    private Vector2 aimDirection;
    private Vector2 targetPosition;
    private bool canShoot = true;

    public float ReloadTimer { get => reloadTimer; set => reloadTimer = value; }

    void Awake()
    {
        tankColliders = GetComponentsInParent<Collider2D>();
    }

    private void Start()
    {
        OnReloading?.Invoke(currentDelay);
    }

    private void Update()
    {
        if (!canShoot)
        {
            OnCantShoot?.Invoke();
            currentDelay -= Time.deltaTime;
            OnReloading?.Invoke(currentDelay);

            if (currentDelay <= 0)
            {
                canShoot = true;
                OnCanShoot?.Invoke();
            }
        }

    }

    void FixedUpdate()
    {
        // tank body rotation
        rb.velocity = (Vector2)transform.up * movementVector.y * moveSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(transform.rotation * Quaternion.Euler(0, 0, -movementVector.x * turnSpeed * Time.fixedDeltaTime));
        
        // turret rotation
        if (GetComponent<PlayerInputHandler>() != null) // is player
        {
            if (PlayerInputHandler.IsGamepad)
            {
                float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                RotateToAngle(targetAngle);
            }
            else
            {
                Vector3 turretDirection = (Vector3)aimDirection - transform.position;
                float targetAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg;
                RotateToAngle(targetAngle);
            }
        }
        else // is enemy
        {
            Vector3 turretDirection = (Vector3)targetPosition - transform.position;
            float targetAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg;
            RotateToAngle(targetAngle);
        }
        
    }

    private void RotateToAngle(float angle)
    {
        float rotationStep = turretSpeed * Time.deltaTime;
        turretBase.rotation = Quaternion.RotateTowards(turretBase.rotation, Quaternion.Euler(0, 0, angle), rotationStep);
    }

    public void HandleBodyMovement(Vector2 moveVector)
    {
        movementVector = moveVector;
    }

    public void HandleTurretRotation(Vector2 aimDir)
    {
        aimDirection = aimDir;        
    }

    public void HandleEnemyTurretRotation(Vector2 targetPos)
    {
        targetPosition = targetPos;
    }

    public void HandleShoot()
    {
        if (canShoot)
        {
            canShoot = false;
            currentDelay = ReloadTimer;

            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.SetParent(FindObjectOfType<ObjectHolder>().transform);
            projectile.transform.position = projectileSpawn.position;
            projectile.transform.localRotation = projectileSpawn.rotation;
            projectile.GetComponent<Projectile>().Initialize();

            foreach (var collider in tankColliders)
            {
                Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), collider);
            }

            OnShoot?.Invoke();
            OnReloading?.Invoke(currentDelay);
        }

    }
}
