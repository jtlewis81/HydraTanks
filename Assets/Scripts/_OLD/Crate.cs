using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField]
    private GameObject crate;

    [SerializeField]
    private GameObject[] pickups;

    [SerializeField]
    private float respawnTime = 15f;

    private float respawnCounter;

    private BoxCollider2D bc;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        ResetTimer();
    }

    private void Update()
    {
        if (!crate.activeSelf)
        {
            respawnCounter -= Time.deltaTime;

            if (respawnCounter <= 0)
            {
                crate.SetActive(true);
                bc.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (crate.activeSelf)
        {
            if (collider.gameObject.tag == "Projectile")
            {
                GameObject drop = Instantiate(pickups[(int)Random.Range(0, pickups.Length)]);
                drop.transform.position = transform.position;
                ResetTimer();
                crate.SetActive(false);
                bc.enabled = false;

            }
        }

    }

    private void ResetTimer()
    {
        respawnCounter = respawnTime;
    }
}
