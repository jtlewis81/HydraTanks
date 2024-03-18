using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
///     Attached to every tank object.
///     It contains references to tank parts, the projectile prefab,
///       settings for the tank's movement, upgrade stats,
///       and Unity Events that can have other scripts attached to them in the Unity editor.
/// 
/// </summary>

public class TankController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform turretBase;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private int range = 6;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turretSpeed;
    [SerializeField] private float reloadTimer;
    [SerializeField] private int hpUpgradeModifier = 25; // should be more than 1, but don't go nuts (additive)
    [SerializeField] private float reloadSpeedModifier = 0.87f; // should be less than, but close to 1 (reduction multiplier)
    [SerializeField] private float moveSpeedModifier = 1.1f; // should be more than, but close to 1 (incremental multiplier)
    [SerializeField] private float turnSpeedModifier = 1.1f; // should be more than, but close to 1 (incremental multiplier)
    [SerializeField] private float turretSpeedModifier = 1.1f; // should be more than, but close to 1 (incremental multiplier)
    [SerializeField] private float damageModifier = 1.2f; // should be more than 1, but don't go crazy (incremental multiplier)
    [SerializeField] private int upgradeRankCap = 5;

    [Header("Editor Info [ DO NOT MODIFY! ]")]
    [SerializeField] private float currentDelay;
    [SerializeField] private int reloadSpeedRank = 1;
    [SerializeField] private int moveSpeedRank = 1;
    [SerializeField] private int aimSpeedRank = 1;
    [SerializeField] private int damageRank = 1;

    [Header("Events")]
    public UnityEvent OnShoot, OnCanShoot, OnCantShoot;
    public UnityEvent<float> OnReloading;

    private CinemachineVirtualCamera vCam;
    private Collider2D[] tankColliders;

    // cached values
    private Vector2 movementVector;
    private Vector2 aimDirection;
    private Vector2 targetPosition;
    private bool canShoot = true;

    // tank upgrade stat public accessors
    public int HPUpgradeModifier { get => hpUpgradeModifier; }
    public int ReloadSpeedRank { get => reloadSpeedRank; set => reloadSpeedRank = value; }
    public int MoveSpeedRank { get => moveSpeedRank; set => moveSpeedRank = value; }
    public int AimSpeedRank { get => aimSpeedRank; set => aimSpeedRank = value; }
    public int DamageRank { get => damageRank; set => damageRank = value; }
    public int UpgradeRankCap => upgradeRankCap;

    void Awake()
    {
        // cache this tank's colliders so we can ignore them with the projectile for the physics system
        tankColliders = GetComponentsInParent<Collider2D>();
    }

    private void Start()
    {
        OnReloading?.Invoke(currentDelay); // trigger the unity event for any listeners to call their subscribed methods

        if(GetComponent<PlayerInputHandler>() != null) // this is for a player tank
        {
            // set this to the virtual camera's follow target
            vCam = FindAnyObjectByType<CinemachineVirtualCamera>();
            vCam.m_Follow = transform;
        }
    }

    // runs every frame
    private void Update()
    {
        if (!canShoot) // canShoot is set to false as soon as the tank fires
        {
            // count down the reload timer
            OnCantShoot?.Invoke();
            currentDelay -= Time.deltaTime;
            OnReloading?.Invoke(currentDelay);

            // allow the tank to fire
            if (currentDelay <= 0)
            {
                canShoot = true;
                OnCanShoot?.Invoke();
            }
        }
    }

    // runs a set number of frames per second according to Unity settings
    // best for object movement
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

    /// <summary>
    /// 
    ///     Helper method for turret rotation
    /// 
    /// </summary>
    /// <param name="angle"></param>    
    private void RotateToAngle(float angle)
    {
        float rotationStep = turretSpeed * Time.deltaTime;
        turretBase.rotation = Quaternion.RotateTowards(turretBase.rotation, Quaternion.Euler(0, 0, angle), rotationStep);
    }

    /// <summary>
    /// 
    ///     Assigned to the PlayerInput/EnemyAI script Unity Events in the Unity editor.
    /// 
    /// </summary>
    /// <param name="moveVector"></param>
    public void HandleBodyMovement(Vector2 moveVector)
    {
        movementVector = moveVector;
    }

    /// <summary>
    /// 
    ///     Assigned to the PlayerInput/EnemyAI script Unity Events in the Unity editor.
    /// 
    /// </summary>
    /// <param name="aimDir"></param>
    public void HandleTurretRotation(Vector2 aimDir)
    {
        aimDirection = aimDir;        
    }

    /// <summary>
    /// 
    ///     Assigned to the PlayerInput/EnemyAI script Unity Events in the Unity editor.
    /// 
    /// </summary>
    /// <param name="targetPos"></param>
    public void HandleEnemyTurretRotation(Vector2 targetPos)
    {
        targetPosition = targetPos;
    }

    /// <summary>
    /// 
    ///     Assigned to the PlayerInput/EnemyAI script Unity Events in the Unity editor.
    /// 
    /// </summary>
    public void HandleShoot()
    {
        if (canShoot)
        {
            canShoot = false;
            currentDelay = reloadTimer;

            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.SetParent(FindObjectOfType<ObjectHolder>().transform);
            projectile.transform.position = projectileSpawn.position;
            projectile.transform.localRotation = projectileSpawn.rotation;
            projectile.GetComponent<Projectile>().Initialize(damage, range);

            // ignore self
            foreach (var collider in tankColliders)
            {
                Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), collider);
            }

            OnShoot?.Invoke();
            OnReloading?.Invoke(currentDelay);
        }

    }

    /// <summary>
    /// 
    ///     Called by the Pickup script to facilitate player upgrades (enemies cannot get upgrades)
    /// 
    /// </summary>
    public void UpgradeReloadSpeed()
    {
        ReloadSpeedRank++;
        reloadTimer *= reloadSpeedModifier;
    }

    /// <summary>
    /// 
    ///     Called by the Pickup script to facilitate player upgrades (enemies cannot get upgrades)
    /// 
    /// </summary>
    public void UpgradeMoveSpeed()
    {
        MoveSpeedRank++;
        moveSpeed *= moveSpeedModifier;
        turnSpeed *= turnSpeedModifier;
    }

    /// <summary>
    /// 
    ///     Called by the Pickup script to facilitate player upgrades (enemies cannot get upgrades)
    /// 
    /// </summary>
    public void UpgradeAimSpeed()
    {
        AimSpeedRank++;
        turretSpeed *= turretSpeedModifier;
    }

    /// <summary>
    /// 
    ///     Called by the Pickup script to facilitate player upgrades (enemies cannot get upgrades)
    /// 
    /// </summary>
    public void UpgradeDamage()
    {
        DamageRank++;
        damage *= damageModifier;
    }
}
