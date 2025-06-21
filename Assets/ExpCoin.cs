using UnityEngine;

public class ExpCoin : MonoBehaviour
{
    public float expValue = 10f; // The amount of experience this coin gives when collected
    private SpawnManager spawnManager;
    private PlayerController playerController; // Reference to the PlayerController script  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>(); // Find the PlayerController in the scene
        spawnManager = GameObject.FindFirstObjectByType<SpawnManager>(); // Find the SpawnManager in the scene
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerController.currentExp >= playerController.maxExp) // Check if the player has reached max experience
        {
            return; // If so, do nothing
        }
        if (collision.CompareTag("Player")) // Check if the collider is the player
        {
           playerController.UpdateExp(expValue); // Call the method to update experience in PlayerController
            Destroy();
        }
    }
    private void Destroy()
    {
        spawnManager.DeactivatePooledObject(gameObject); // Deactivate the coin object and return it to the pool
        transform.position = Vector3.zero; // Reset position to zero
    }
}
