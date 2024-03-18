using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
///		Attach to a projectile prefab.
///		Controls the movement of a fired projectile
///		  and what happens when it hits something.
/// 
/// </summary>

public class Projectile : MonoBehaviour
{
	[SerializeField] private Rigidbody2D rb; // reference to the rigidbody component the physics system uses to move the projectile
	[SerializeField] private float speed = 10f; // setting for how fast the projectile should move

	// these are set when an instance of this projectile is instantiated
	private int range;
    private float damage;
    private Vector2 spawnPosition;
	private float distanceTravelled;

	// used to activate VFX and SFX when the projectile hits something
	public UnityEvent OnHit = new UnityEvent();

	public void Initialize(float damage, int range)
	{
		this.damage = damage;
		this.range = range;
		spawnPosition = transform.position;
		rb.velocity = transform.up * speed;
	}

	private void Update()
	{
		// diable the projectile without a hit after the range is reached
		distanceTravelled = Vector2.Distance(transform.position, spawnPosition);
		if(distanceTravelled >= range)
		{
			Disable();
		}
	}
	
	/// <summary>
	/// 
	///		Helper method to stop and destroy this GameObject 
	/// 
	/// </summary>
	private void Disable()
	{
		rb.velocity = Vector2.zero;
		Destroy(gameObject);
	}

	/// <summary>
	/// 
	///		A built in Unity MonoBehaviour method for handling physics collisions.
	///		Detects what this object collided with.
	/// 
	/// </summary>
	/// <param name="collider"></param>
	private void OnTriggerEnter2D(Collider2D collider)
	{
		// fire the Unity Event 
		OnHit?.Invoke();

		// see if the hit object has a damageable script that shoud be accessed
		var damageable = collider.GetComponent<Damageable>();
		if (damageable != null)
		{
			damageable.Hit(damage); // damage the hit object
		}

		Disable(); // stop and destroy this object
	}
}
