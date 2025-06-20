using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public bool wantToSpawn;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

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
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.identity;
        poolDictionary[tag].Enqueue(objectToSpawn); // Re-add the object to the pool
    }

    public void DeactivatePooledObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
