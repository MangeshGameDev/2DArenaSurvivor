using UnityEngine;
using System.Collections;

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance;
    private SpawnManager spawnManager;
    public PlayerController playerController; 

    public int waveNumber = 0;
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;
    public float spawnDelay = 0.3f; // Delay between each enemy spawn
    private float waveTimer = 0f;
    private string enemyTag = "Enemy";
    [HideInInspector]
    public int enemyCount = 0;
    private bool waitingForNextWave = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        spawnManager = GetComponent<SpawnManager>();
        if(spawnManager == null)
        {
            Debug.LogError("SpawnManager not found on WaveSystem. Please attach a SpawnManager component.");
            return;
        }
        
        if (playerController.isDead == false) { StartNextWave(); }
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
        StartCoroutine(SpawnWaveCoroutine(enemyCount));
        // update the max experience in PlayerController
         playerController.UpdateMaxExp();
    }

    IEnumerator SpawnWaveCoroutine(int count)
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float xMin = cam.transform.position.x - camWidth / 2f;
        float xMax = cam.transform.position.x + camWidth / 2f;
        float yMin = cam.transform.position.y - camHeight / 2f;
        float yMax = cam.transform.position.y + camHeight / 2f;

        float spawnBuffer = 2f;

        for (int i = 0; i < count; i++)
        {
            int edge = Random.Range(0, 4);
            Vector2 spawnPosition = Vector2.zero;

            switch (edge)
            {
                case 0: // Left
                    spawnPosition = new Vector2(xMin - spawnBuffer, Random.Range(yMin, yMax));
                    break;
                case 1: // Right
                    spawnPosition = new Vector2(xMax + spawnBuffer, Random.Range(yMin, yMax));
                    break;
                case 2: // Top
                    spawnPosition = new Vector2(Random.Range(xMin, xMax), yMax + spawnBuffer);
                    break;
                case 3: // Bottom
                    spawnPosition = new Vector2(Random.Range(xMin, xMax), yMin - spawnBuffer);
                    break;
            }

            spawnManager.SpawnFromPool(enemyTag, spawnPosition);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

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
