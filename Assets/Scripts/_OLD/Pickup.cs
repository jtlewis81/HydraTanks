using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
	[SerializeField]
	private float timeToLive = 10f;

     private enum PickupType { MaxHP, Repair, FireRate };

     [SerializeField]
     private PickupType type;

	[SerializeField]
	private float HPUpgradeModifier = 1.25f; // should be more than 1, but don't go nuts
	[SerializeField]
	private float fireRateModifier = 0.9f; // should be less than, but close to 1

	private GameObject player;
	private Damageable playerHealth;
	private TankController playerController;

	private void Awake()
	{
		player = FindObjectOfType<PlayerInputHandler>().gameObject;
		playerHealth = player.GetComponent<Damageable>();
		playerController = player.GetComponent<TankController>();
	}

	private void Update()
	{
		timeToLive -= Time.deltaTime;

		if (timeToLive <= 0)
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			// if any more upgrades are added, consider changing to a switch method
			if (type == PickupType.MaxHP)
			{
				playerHealth.MaxHP = (int)(playerHealth.MaxHP * HPUpgradeModifier);
				playerHealth.Heal((playerHealth.MaxHP - playerHealth.CurrHP) / 2);
				Hud.Instance.UpdateHP();
				Destroy(gameObject);
			}
			else if (type == PickupType.Repair)
			{
				playerHealth.Heal(playerHealth.MaxHP);
                Hud.Instance.UpdateHP();
				Destroy(gameObject);
			}
			else if (type == PickupType.FireRate)
			{
				playerController.ReloadTimer *= fireRateModifier;
				Hud.Instance.UpdateFireRate();
				Destroy(gameObject);
			}
		}
		
	}
}
