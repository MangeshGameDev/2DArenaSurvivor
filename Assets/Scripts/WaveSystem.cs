using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public SpawnManager spawnManager;
    public int waveNumber = 0; // Current wave number
    public int enemiesPerWave = 5; // Number of enemies to spawn per wave
    public float timeBetweenWaves = 5f; // Time between waves
    private float waveTimer = 1f;
    private string enemyTag = "Enemy"; // Tag for enemy pool

    void Start()
    {
        spawnManager = GetComponent<SpawnManager>();
        StartNextWave();
    }

    void Update()
    {
        if (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                StartNextWave();
            }
        }
    }

    void StartNextWave()
    {
        waveNumber++;
        int enemiesToSpawn = enemiesPerWave + (waveNumber - 1) * 2; // Increase difficulty by 2 enemies per wave

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector2 spawnPosition = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            spawnManager.SpawnFromPool(enemyTag, spawnPosition);
        }

        waveTimer = timeBetweenWaves;
    }
}
