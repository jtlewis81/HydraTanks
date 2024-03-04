using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]

    [SerializeField]
    private Transform target;
    [SerializeField]
    private float pathUpdateSeconds = 0.5f;
    [SerializeField]
    private float nextWaypointDistance = 0.5f;

    private Rigidbody2D rb;
    private PlayerDetector playerDetector;

    private float distance;
    private Path path;
    private int currentWaypoint = 0;
    private Seeker seeker;

    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnAimTurret = new UnityEvent<Vector2>();

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
        seeker = GetComponentInChildren<Seeker>();

    }

    private void Start()
    {
        if (!LevelManager.Instance.IsGameOver)
        {
            target = FindObjectOfType<PlayerInputHandler>().transform; // needs updated for multiplayer

            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        }
    }

    void Update()
    {
        if (!LevelManager.Instance.IsGameOver)
        {
            if (target != null)
            {
                OnAimTurret?.Invoke((Vector2)target.position);
            }

            // shoot || move (not both in same frame)
            if (playerDetector.TargetVisible)
            {
                OnMoveBody?.Invoke(Vector2.zero);
                OnShoot?.Invoke();
            }
            else
            {
                // pathfinding stuff

                if (path == null || target == null)
                {
                    return;
                }

                if (currentWaypoint >= path.vectorPath.Count)
                {
                    return;
                }

                distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }

                if (currentWaypoint >= path.vectorPath.Count)
                {
                    return;
                }

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
        }
        else
        {
            OnMoveBody?.Invoke(Vector2.zero);
        }
    }



    // PATHFINDING STUFF

    private void UpdatePath()
    {
        if (target != null && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
