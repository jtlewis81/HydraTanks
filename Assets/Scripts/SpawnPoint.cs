using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
///     Attached to an empty GameObject in the Scene heirarchy. 
///     Handles spawning enemy tanks.
///     The LevelManager adds tanks to the queue.
/// 
/// </summary>

public class SpawnPoint : MonoBehaviour
{
    private enum spawnRotation { N, S, E, W }; // used to set the direction a tank should face when it spawns

    // Serialized Fields are visible in the Unity editor's inspector tab
    [SerializeField] private spawnRotation spawnFacingDirection; // set the direction a tank should face when it spawns
    [SerializeField] private GameObject[] tankPrefabs; // a list of the possible prefabs this spawner should

    // how long in seconds before the next tank in the queue is spawned after it reaches the front of the queue
    // set to 0 in the editor for player spawners
    [SerializeField] private float spawnDelayInit = 2f; 

    private float spawnDelay; // used to track the current delay time
    private Queue<GameObject> spawnQueue; // a queue for spawning tanks from this spawner
    
    // runs once when the object with this script is loaded into the game
    void Awake()
    {
        // create a new ojbect queue
        spawnQueue = new Queue<GameObject>();
        
        // set the timer to the set delay amount
        ResetSpawnDelay();
    }

    void Update()
    {
        // only count down the timer if there is a tank in the queue
        if (spawnQueue != null && spawnQueue.Count > 0)
        {
            if (spawnDelay > 0)
            {
                spawnDelay -= Time.deltaTime;
            }
            else if (spawnDelay <= 0) // timer expired
            {
                // instantiate a tnak into the world
                GameObject tank = Instantiate(spawnQueue.Dequeue());

                // if this is an enemy tank, add it to the LevelManager's counter toward the level's enemy cap
                if(tank.GetComponent<EnemyAI>() != null)
                {
                    LevelManager.Instance.EnemyCount++;
                }

                // set the tank's properties
                tank.transform.SetParent(FindObjectOfType<ObjectHolder>().transform); // set parent to the level's object holder so it gets destroyed when the level is unloaded
                tank.transform.position = transform.position; // put the tank where the spawner is
                SetRotation(tank); // set the rotation according to the spawnRotaion setting
                ResetSpawnDelay(); // reset the queue's delay timer
            }
        }
    }

    /// <summary>
    /// 
    ///     Sets the tank's rotation when spawning it.
    /// 
    /// </summary>
    /// <param name="tank"></param>
    /// <returns></returns>
    private GameObject SetRotation(GameObject tank)
    {

        if (spawnFacingDirection == spawnRotation.N)
        {
            tank.transform.Rotate(0, 0, 0);
        }
        else if (spawnFacingDirection == spawnRotation.S)
        {
            tank.transform.Rotate(0, 0, 180);
        }
        else if (spawnFacingDirection == spawnRotation.E)
        {
            tank.transform.Rotate(0, 0, -90);
        }
        else if (spawnFacingDirection == spawnRotation.W)
        {
            tank.transform.Rotate(0, 0, 90);
        }

        return tank;
    }

    /// <summary>
    /// 
    ///     Called by the LevelManager to add a tank to the queue for spawning.
    /// 
    /// </summary>
    public void AddToQueue()
    {
        int index = 0;
        if (tankPrefabs.Length > 1)
        {
            int random = Random.Range(1, 101);
            if(random <= LevelManager.Instance.HarderEnemyChance)
            {
                index = 1;
            }
        }

        spawnQueue.Enqueue(tankPrefabs[index]);
    }

    /// <summary>
    /// 
    ///     Helper method for resetting the spawn delay.
    /// 
    /// </summary>
    private void ResetSpawnDelay()
    {
        spawnDelay = spawnDelayInit;
    }

}
