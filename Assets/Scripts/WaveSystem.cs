using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public SpawnManager spawnManager;
    public int waveNumber = 0;
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;
    private float waveTimer = 0f;
    private string enemyTag = "Enemy";
    private int enemyCount = 0;
    private bool waitingForNextWave = false;

    void Start()
    {
        spawnManager = GetComponent<SpawnManager>();
        StartNextWave();
    }

    void Update()
    {
        if (waitingForNextWave)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                waitingForNextWave = false;
                StartNextWave();
            }
        }
    }

    void StartNextWave()
    {
        waveNumber++;
        enemyCount = enemiesPerWave + (waveNumber - 1) * 2;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 spawnPosition = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            spawnManager.SpawnFromPool(enemyTag, spawnPosition);
        }
    }

    // Call this from your enemy script when an enemy dies
    public void OnEnemyDied()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            waveTimer = timeBetweenWaves;
            waitingForNextWave = true;
        }
    }
}
