using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpanwer : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;



    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 10;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBeforeWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroyed = new UnityEvent();
    public static UnityEvent<GameObject> enemySpawn = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> enemyDestroyed = new UnityEvent<GameObject>();



    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private void Awake()
    {
        onEnemyDestroyed.AddListener(EnemyDestroyed);

    }

    // Start is called before the first frame update
    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBeforeWaves);
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        currentWave++;  // Move to the next wave
    }

    private void Start()
    {
        StartCoroutine(StartWave());     
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

    // Update is called once per frame
    void Update()
    {
        if(!isSpawning)
        {
            return;
        }

        timeSinceLastSpawn += Time.deltaTime;
        if(timeSinceLastSpawn >= 1f/enemiesPerSecond && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if(enemiesAlive == 0 &&  enemiesLeftToSpawn == 0)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        StartCoroutine(StartWave());
        currentWave++;

    }

    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = enemyPrefabs[0];
        GameObject enemy = Instantiate(prefabToSpawn,LevelManager.main.startPoint.position,Quaternion.identity);   
        enemySpawn.Invoke(enemy);

    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
        enemyDestroyed.Invoke(gameObject);  // Assuming `gameObject` is the destroyed enemy
    }

}
