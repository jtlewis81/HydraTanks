using System.Collections;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
	[Range(1, 15)]
	[SerializeField] private float targetRange = 5;
	[SerializeField] private float checkFrequency = 0.1f;
	[SerializeField] private Transform target;
	[SerializeField] private LayerMask playerLM;

	private new Collider2D collider;
	private RaycastHit2D[] hitObjects = new RaycastHit2D[1];

    [field: SerializeField]
	public bool TargetVisible { get; private set;  }
	public Transform Target
	{
		get => target;
		set
		{
			target = value;
			TargetVisible = false;
		}
	}

	public float TargetRange { get => targetRange; set => targetRange = value; }

    private void Awake()
    {
        collider = GetComponentInParent<Collider2D>();
    }

    private void Start()
	{
		StartCoroutine(DetectionCoroutine());
	}

	private void Update()
	{
		if (Target != null)
		{
			TargetVisible = CheckTargetVisibility();
		}
	}

	private bool CheckTargetVisibility()
	{
        int hits = collider.Raycast(Target.position - transform.position, hitObjects, TargetRange, playerLM);
        if (hits > 0)
        {
			return hitObjects[0].collider.gameObject.CompareTag("Player");
		}
		return false;
	}


	private void DetectTarget()
	{
		if (Target == null)
		{
			CheckIfPlayerInRange();
		}
		else if (Target != null)
		{
			DetectIfOutOfRange();
		}
	}

	private void DetectIfOutOfRange()
	{
		if (Target == null || !Target.gameObject.activeSelf || Vector2.Distance(transform.position, Target.position) > TargetRange)
		{
			Target = null;
		}
	}

	private void CheckIfPlayerInRange()
	{
		Collider2D collision = Physics2D.OverlapCircle(transform.position, TargetRange, playerLM);
		if (collision != null)
		{
			Target = collision.transform;
		}
	}

	IEnumerator DetectionCoroutine()
	{
		yield return new WaitForSeconds(checkFrequency);
		DetectTarget();
		StartCoroutine(DetectionCoroutine());
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, TargetRange);
	}
}
