using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject tankPrefab;

    [SerializeField]
    private float spawnDelayInit = 2f;

    private float spawnDelay;

    private Queue<GameObject> spawnQueue;

    private enum spawnRotation { N, S, E, W };

    [SerializeField]
    private spawnRotation spawnFacingDirection;

    public UnityEvent<Transform> OnSpawnTank = new UnityEvent<Transform>();


    // Start is called before the first frame update
    void Awake()
    {
        spawnQueue = new Queue<GameObject>();
        ResetSpawnDelay();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnQueue != null && spawnQueue.Count > 0)
        {
            if (spawnDelay > 0)
            {
                spawnDelay -= Time.deltaTime;
            }
            else if (spawnDelay <= 0)
            {
                GameObject tank = Instantiate(spawnQueue.Dequeue());
                if(tank.GetComponent<EnemyAI>() != null)
                {
                    LevelManager.Instance.EnemyCount++;
                }
                tank.transform.SetParent(FindObjectOfType<ObjectHolder>().transform);
                tank.transform.position = transform.position;
                SetRotation(tank);
                ResetSpawnDelay();
            }
        }
    }

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

    public void AddToQueue()
    {
        spawnQueue.Enqueue(tankPrefab);
    }

    private void ResetSpawnDelay()
    {
        spawnDelay = spawnDelayInit;
    }

}
