using System.Collections;
using UnityEngine;

/// <summary>
/// 
///		Attached to a child object on an enemy tank.
///		Used by the EnemyAI to assist in shooting.
///		The target in question here is for the turret to shoot at, not for the tank to chase.
///		This script is designed with a future addition of multi-player in mind.
/// 
/// </summary>

public class PlayerDetector : MonoBehaviour
{
	// settings
	[Range(1, 15)]
	[SerializeField] private float targetRange = 5;			// generally needs to match the tank's range setting, but could be different for whatever reason
	[SerializeField] private float checkFrequency = 0.1f;   // determines how many time per second that the coroutine runs to scan for the target
	[SerializeField] private Transform target;				// reference to the target gets set when the tank is spawned
	[SerializeField] private LayerMask playerLM;			// filter for the scan

	// cached data
	private new Collider2D collider;
	private RaycastHit2D[] hitObjects = new RaycastHit2D[1];

	// field attribute added so we can see public property in the editor
    [field: SerializeField]	public bool TargetVisible { get; private set; } // referenced in EnemyAI
	
	// this could be private for now, technically could just use the reference above to "target",
	//   but will liklely need to be public when I implement multi-player, or for a more advanced AI implementation
	// also sets the target visibility to false immediately when changing targets, that can get checked again separately
	public Transform Target
	{
		get => target;
		set
		{
			target = value;
			TargetVisible = false;
		}
	}

	// could technically be eliminated for now, but same reasoning as for Target
	public float TargetRange { get => targetRange; set => targetRange = value; }

    private void Awake()
    {
        collider = GetComponentInParent<Collider2D>();
    }

    private void Start()
	{
		StartCoroutine(DetectionCoroutine()); // runs a coroutine using the checkFrequency as a kind of timer system so it doesn't check every frame (better performance)
	}

	private void Update()
	{
		// constantly check if there is a direct line of sight to the target
		if (Target != null)
		{
			TargetVisible = CheckTargetVisibility();
		}
	}

    /// <summary>
    /// 
    ///		Performs a raycast to check if there is a direct line of sight to the target
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CheckTargetVisibility()
	{
        int hits = collider.Raycast(Target.position - transform.position, hitObjects, TargetRange);
        if (hits > 0)
        {
			return hitObjects[0].collider.gameObject.CompareTag("Player");
		}
		return false;
	}

	IEnumerator DetectionCoroutine()
	{
		yield return new WaitForSeconds(checkFrequency);
		DetectTarget();
		StartCoroutine(DetectionCoroutine());
	}

	/// <summary>
	/// 
	///		Called by the coroutine.
	///		Checks if the target is still in range to shoot at.
	/// 
	/// </summary>
	private void DetectTarget()
	{
		if (Target == null) // currently can't see target
		{
			CheckIfPlayerInRange(); // look for a player within range
		}
		else if (Target != null) // currently has a target
		{
			DetectIfOutOfRange(); // 
		}
	}

	/// <summary>
	/// 
	///		Attempts to set a valid target.
	/// 
	/// </summary>
	private void CheckIfPlayerInRange()
	{
		Collider2D collision = Physics2D.OverlapCircle(transform.position, TargetRange, playerLM); // physics check for valid target
		if (collision != null)
		{
			Target = collision.transform;
		}
	}

	/// <summary>
	/// 
	///		See if target is still valid.
	/// 
	/// </summary>
	private void DetectIfOutOfRange()
	{
		if (Target == null || !Target.gameObject.activeSelf || Vector2.Distance(transform.position, Target.position) > TargetRange) // target may have been destroyed or gotten out of range
		{
			Target = null;
		}
	}

	/// <summary>
	/// 
	///		Draws a circle representing the range setting in the editor's scene view.
	/// 
	/// </summary>
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, TargetRange);
	}
}
