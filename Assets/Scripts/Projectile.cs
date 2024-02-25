using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D rb;

	[SerializeField]
	private float speed;
	[SerializeField]
	private int damage;
	[SerializeField]
	private float range;

	private Vector2 spawnPosition;
	private float distanceTravelled;

	public UnityEvent OnHit = new UnityEvent();

	public void Initialize()
	{
		spawnPosition = transform.position;
		rb.velocity = transform.up * speed;
	}

	private void Update()
	{
		distanceTravelled = Vector2.Distance(transform.position, spawnPosition);
		if(distanceTravelled >= range)
		{
			Disable();
		}
	}
	
	private void Disable()
	{
		rb.velocity = Vector2.zero;
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		OnHit?.Invoke();
		var damageable = collider.GetComponent<Damageable>();
		if (damageable != null)
		{
			damageable.Hit(damage);
		}

		Disable();
	}
}
