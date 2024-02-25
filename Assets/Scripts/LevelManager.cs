using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Object References")]
    [SerializeField] private SpawnPoint[] enemySpawnpoints;
    [SerializeField] private SpawnPoint[] playerSpawnpoints;

    [Header("Settings")]
    [SerializeField] private float gameOverDelay = 1f;
    [SerializeField] private int enemyCap = 8;


    [Header("Editor Info [ DO NOT CHANGE ]")]
    [SerializeField] private int killCount = 0;

    public int EnemyCount { get; set; }
    public int KillCount { get => killCount; set => killCount = value; }

    public bool IsGameOver = false;

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


        EnemyCount = 0;
    }

    void Start()
    {
        SpawnPlayers();

        // spawn a single enemy to start with
        int random = Random.Range(1, enemySpawnpoints.Length + 1) - 1;
        enemySpawnpoints[random].AddToQueue();
    }

    private void SpawnPlayers()
    {
        int random = Random.Range(1, playerSpawnpoints.Length + 1) - 1;
        playerSpawnpoints[random].AddToQueue();
    }

    public void SpawnEnemyTanks()
    {

        for (var i = 0; i < 2; i++)
        {
            if (EnemyCount < enemyCap)
            {
                int random = Random.Range(1, enemySpawnpoints.Length + 1) - 1;
                enemySpawnpoints[random].AddToQueue();
            }
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
        Invoke("OpenGameOverScreenAfterDelay", gameOverDelay);
        // run our save system to add score to high scores
    }

    private void OpenGameOverScreenAfterDelay()
    {
        MenuSystem.Instance.Score = KillCount;
        MenuSystem.Instance.GameOver();
    }

    public void HandleEnemyDestoyed()
    {
        EnemyCount--;
        KillCount++;
        Hud.Instance.UpdateTanksDestroyed();
        SpawnEnemyTanks();
    }

}
