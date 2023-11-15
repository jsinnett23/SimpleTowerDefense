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
    public static UnityEvent<GameObject> onEnemyDestroyed = new UnityEvent<GameObject>();  // Corrected to match the listener's signature
    public static UnityEvent<GameObject> enemySpawn = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> enemyDestroyed = new UnityEvent<GameObject>();



    public static int currentWave = 1; // Make this public and static
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
        Debug.Log("Starting Wave: " + currentWave + " with " + enemiesLeftToSpawn + " enemies to spawn"); // Debug log for start of wave

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
            Debug.Log("Spawned Enemy. Enemies Left to Spawn: " + enemiesLeftToSpawn + ", Enemies Alive: " + enemiesAlive); // Debug log for enemy spawn

        }

        if (enemiesAlive == 0 &&  enemiesLeftToSpawn == 0)
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
        LevelManager.main.UpdateUI(); // Update UI with new wave number


        Debug.Log("Ended Wave: " + (currentWave - 1)); // Debug log for end of wave


    }

    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = enemyPrefabs[0];
        GameObject enemy = Instantiate(prefabToSpawn,LevelManager.main.startPoint.position,Quaternion.identity);   
        enemySpawn.Invoke(enemy);

    }

    private void EnemyDestroyed(GameObject destroyedEnemy)
    {
        enemiesAlive--;
        Debug.Log("Enemy Destroyed. Enemies Alive: " + enemiesAlive);

        if (enemiesAlive < 0)
        {
            Debug.LogError("Enemies Alive went negative! This shouldn't happen.");
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0 && isSpawning)
        {
            EndWave();
        }
    }

}
