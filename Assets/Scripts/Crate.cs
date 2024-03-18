using UnityEngine;

/// <summary>
/// 
///     Attached to the Crate prefab.
///     Handles tempoarily diabling the crate when a player shoots it and
///       dropping a random pickup for the player.
/// 
/// </summary>

public class Crate : MonoBehaviour
{
    // game object references
    [SerializeField] private GameObject crate; // references itself. could also be set via code in Awake()
    [SerializeField] private GameObject[] pickups; // a list of the pickups this crate chould have avaailable to pick from randomly.

    // cached refence to the attached box collider. set in Awake()
    // we need to be able to enable/disable this to make collisions and enemy pathfinding work as desired
    private BoxCollider2D bc;

    // cached values
    private float respawnTime; // this is set in Start via a setting in the LevelManager.
    private float respawnCounter;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // set the respawn timer to the setting in the LevelManager
        respawnTime = LevelManager.Instance.UpgradeRespawnDelay;
        ResetTimer();
    }

    private void Update()
    {
        // turn the crate back on after the timer expires
        if (LevelManager.Instance.CanSpawnCrates && !crate.activeSelf)
        {
            respawnCounter -= Time.deltaTime;

            if (respawnCounter <= 0)
            {
                crate.SetActive(true);
                bc.enabled = true;
                LevelManager.Instance.UpdateMap(bc);
            }
        }
    }

    /// <summary>
    /// 
    ///     Collission check for a projectile.
    ///     Turns the Crate off and drops a pickup.
    /// 
    /// </summary>
    /// <param name="collider"></param>
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
                LevelManager.Instance.UpdateMap(bc);
                bc.enabled = false;
            }
        }
    }

    /// <summary>
    /// 
    ///     Helper method to reset the respawn timer.
    /// 
    /// </summary>
    private void ResetTimer()
    {
        respawnCounter = respawnTime;
    }
}
