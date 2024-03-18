using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

/// <summary>
///
///     Handles making the enemy tanks track and shoot at a player.
///     Utilizes the free version of the A* Pathfinding Project from:  https://arongranberg.com/astar.
///     Utilizes the PlayerDetector that is attached to a child object of the same parent game object.
/// 
/// </summary>

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] private Transform target;                  // for reference in the inspector only. the target is set when the tank is instantiated
    [SerializeField] private float pathUpdateSeconds = 0.5f;    // how often the pathfinding recalculates
    [SerializeField] private float nextWaypointDistance = 0.5f; // how far apart the points of a path should be. smaller is a shorter & smoother path. longer is more performant/less calculations.

    // cached references
    private Rigidbody2D rb;
    private PlayerDetector playerDetector;
    private Seeker seeker;

    // cached values
    private Path path;
    private int currentWaypoint = 0;
    private float distance;

    // cahced & hardcoded values for handling an enemy if they get stuck in a tight spot of a level map
    private float stuckTimeout = 2f;
    private float stuckTimer = 0f;
    private Vector2 lastPosition;
    private Vector3 lastRotation;
    private bool isStuck = false;

    // Unity Events that trigger TankController methods
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnAimTurret = new UnityEvent<Vector2>();

    private void Awake()
    {
        // cache component refereneces
        rb = GetComponentInChildren<Rigidbody2D>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
        seeker = GetComponentInChildren<Seeker>();
    }

    private void Start()
    {
        // do not initialize targeting & pathfinding if game is over before being spawned, player will be null
        if (!LevelManager.Instance.IsGameOver) // game is not over
        {
            target = FindObjectOfType<PlayerInputHandler>().transform; // needs updated for multiplayer

            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        }

        // cache position and rotation for stuck checks
        lastPosition = transform.position;
        lastRotation = transform.forward;
    }

    void Update()
    {
        if (!LevelManager.Instance.IsGameOver) // game is not over
        {
            if (target != null)
            {
                OnAimTurret?.Invoke((Vector2)target.position);
            }

            // shoot || move (not both in same frame)
            if (playerDetector.TargetVisible)
            {
                // if can see target, stop moving and fire
                OnMoveBody?.Invoke(Vector2.zero);
                OnShoot?.Invoke();
            }
            else
            {
                // check if stuck
                // the stuck checking mechanics migh need a little polishing, but it works. 

                if (!isStuck && stuckTimer < stuckTimeout)
                {
                    stuckTimer += Time.deltaTime;
                }

                if (stuckTimer >= stuckTimeout && Vector2.Distance(lastPosition, transform.position) < .25f)
                {
                    isStuck = true;
                }

                // if stuck, rotate
                if (isStuck)
                {
                    // rotate
                    transform.Rotate(0, 0, -1);
                    if (transform.forward.z - lastRotation.z >= Mathf.Abs(30))
                    {
                        isStuck = false;
                        stuckTimer = 0f;
                        lastPosition = transform.position;
                        lastRotation = transform.forward;
                    }
                }
                else // move normally
                {

                    // pathfinding stuff

                    // should have a path and a target
                    if (path == null || target == null)
                    {
                        return;
                    }

                    // do not exceed bounds of waypoint array
                    if (currentWaypoint >= path.vectorPath.Count)
                    {
                        return;
                    }

                    // get distance to next waypoint of the path
                    distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                    // increment the waypoint
                    if (distance < nextWaypointDistance)
                    {
                        currentWaypoint++;
                    }

                    // the increment should not exceed the waypoint array size 
                    if (currentWaypoint >= path.vectorPath.Count)
                    {
                        return;
                    }

                    // calculate movement toward the current waypoint, which gets passed to the subscribed events in the TankController
                    Vector2 movementVector = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);

                    var dotProduct = Vector2.Dot(transform.up, movementVector.normalized);

                    if (dotProduct < 0.98f)
                    {
                        var crossProduct = Vector3.Cross(transform.up, movementVector.normalized);
                        int rotationResult = crossProduct.z >= 0 ? -1 : 1;
                        OnMoveBody?.Invoke(new Vector2(rotationResult, 1));
                    }
                    else
                    {
                        OnMoveBody?.Invoke(Vector2.up);
                    }
                }

                // update position/rotation if not stuck
                if (Vector2.Distance(lastPosition, transform.position) >= .25f)
                {
                    isStuck = false;
                    stuckTimer = 0f;
                    lastPosition = transform.position;
                    lastRotation = transform.forward;
                }
            }
        }
        else // game is over, don't move
        {
            OnMoveBody?.Invoke(Vector2.zero);
        }
    }

    // PATHFINDING STUFF

    /// <summary>
    /// 
    ///     Called via a Unity Event whenever something on the map changes that should cause an enemy to recalculate its path to the target player.
    /// 
    /// </summary>
    private void UpdatePath()
    {
        if (target != null && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    /// <summary>
    /// 
    ///     Callback method used in the UpdatePath() method above.
    /// 
    /// </summary>
    /// <param name="p"></param>
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
