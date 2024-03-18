using Pathfinding;
using UnityEngine;

/// <summary>
/// 
///     Handles pretty much everything that is specific to a level.
///     Each level has its own LevelManager object in the Scene heirarchy.
/// 
/// </summary>

public class LevelManager : MonoBehaviour
{
    // is a singleton
    public static LevelManager Instance;

    [SerializeField] private int levelNumber; // used for submitting scores

    // manages player and enemy spawning
    [Header("Object References")]
    [SerializeField] private SpawnPoint[] enemySpawnpoints;
    [SerializeField] private SpawnPoint[] playerSpawnpoints;

    [Header("Settings")]
    [SerializeField] private float gameOverDelay = 1f;
    [SerializeField] private float upgradeRespawnDelay = 15f;
    [SerializeField] private int enemyCap = 8;
    [SerializeField] private int harderEnemyChance = 20;

    // tracks score
    [Header("Editor Info [ DO NOT MODIFY! ]")]
    [SerializeField] private int killCount = 0;

    public int EnemyCount { get; set; } // modified in SpawnPoint
    public int KillCount => killCount; // referenced in Hud
    public float UpgradeRespawnDelay => upgradeRespawnDelay; // referenced in Crate
    public int HarderEnemyChance => harderEnemyChance; // referenced in SpawnPoint
    public bool CanSpawnCrates = false; // referenced in Crate
    public bool IsGameOver = false; // referenced in EnemyAI

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // reset enemy count on level load
        EnemyCount = 0;
    }

    void Start()
    {
        SpawnPlayers();

        // spawn a single enemy to start with
        int random = Random.Range(1, enemySpawnpoints.Length + 1) - 1;
        enemySpawnpoints[random].AddToQueue();
    }

    /// <summary>
    /// 
    ///     Spawn player at a random spawn point.
    /// 
    /// </summary>
    private void SpawnPlayers()
    {
        int random = Random.Range(1, playerSpawnpoints.Length + 1) - 1;
        playerSpawnpoints[random].AddToQueue();
    }

    /// <summary>
    /// 
    ///     Spawn enemies at random spawn points.
    /// 
    /// </summary>
    public void SpawnEnemyTanks()
    {
        for (var i = 0; i < 2; i++) // magic number 2 is used as the core game mechanic: killing one enemy spawns two more
        {
            // do not spawn if enemyCap is reached
            if (EnemyCount < enemyCap)
            {
                int random = Random.Range(1, enemySpawnpoints.Length + 1) - 1;
                enemySpawnpoints[random].AddToQueue();
            }
        }
    }

    /// <summary>
    /// 
    ///     Called in player's Damageable component when they are killed.
    /// 
    /// </summary>
    public void GameOver()
    {
        SaveSystem.Instance.SubmitScore(levelNumber, killCount); // submit the final score to the SaveSystem
        IsGameOver = true;
        Invoke("OpenGameOverScreenAfterDelay", gameOverDelay); // wait a second to show the game over screen so the player can see their tank explode.
    }

    /// <summary>
    /// 
    ///     Invoked in GameOver().
    ///     Passes score to the MenuSystem.
    ///     Calls the MenuSystem's GameOver() method, which opens the game over menu screen
    /// 
    /// </summary>
    private void OpenGameOverScreenAfterDelay()
    {
        MenuSystem.Instance.Score = KillCount;
        MenuSystem.Instance.GameOver();
    }

    /// <summary>
    /// 
    ///     Manages enemy count toward the cap.
    ///     Increments score.
    ///     Allows Crate spawning after the first enemy is killed.
    ///     Calls Hud method to update score in the UI.
    ///     Triggers spawning of more enemy tanks.
    /// 
    /// </summary>
    public void HandleEnemyDestoyed()
    {
        EnemyCount--;
        killCount++;
        CanSpawnCrates = true;
        Hud.Instance.UpdateTanksDestroyed();
        SpawnEnemyTanks();
    }

    /// <summary>
    /// 
    ///     A* Pathifnding updates the map at the coordinates of the passed in collider. 
    ///     Manages walkable nodes for EnemyAI.
    ///     Called whenever a Crate turns on or off.
    ///     Called when a destuctible object is destroyed.
    /// 
    /// </summary>
    /// <param name="collider"></param>
    public void UpdateMap(Collider2D collider)
    {
        Debug.Log("[ LEVEL MANAGER ] Updating Map...");

        Bounds bounds = collider.bounds;
        var guo = new GraphUpdateObject(bounds);
        AstarPath.active.UpdateGraphs(guo);
    }
}
