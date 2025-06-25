using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectPoolingStart();
    }

    private void ObjectPoolingStart()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();// Initialize the dictionary to hold the object pools
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();// Create a new queue for each pool
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, this.transform); // Set parent to SpawnManager
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpawnFromPool(string tag,Vector2 position)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();// Get an object from the pool
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.identity;
        objectToSpawn.SetActive(true);

    }

    public void DeactivatePooledObject(GameObject obj)
    {
        if (!poolDictionary.ContainsKey(obj.tag))
        {
            Debug.LogWarning($"No pool with tag {obj.tag} exists.");
            return;
        }
        poolDictionary[obj.tag].Enqueue(obj);
        obj.SetActive(false);
       
       
    }
}
