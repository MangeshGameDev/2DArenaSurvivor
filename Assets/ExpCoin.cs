using UnityEngine;

public class ExpCoin : MonoBehaviour
{
    public float expValue = 10f; 
    private SpawnManager spawnManager;
    private PlayerController playerController; 
    

    void Start()
    {
        playerController = PlayerController.instance; // Find the PlayerController in the scene
        spawnManager = SpawnManager.Instance; // Find the SpawnManager in the scene
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerController.currentExp >= playerController.maxExp) 
        {
            return; // If so, do nothing
        }
        if (collision.CompareTag("Player") && collision is CapsuleCollider2D)
        {
           playerController.UpdateExp(expValue); 
            DisablegameObject();
        }
    }

    private void DisablegameObject()
    {
        spawnManager.DeactivatePooledObject(gameObject); 
        
    }
}
