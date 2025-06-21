using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UpgradeManager : MonoBehaviour
{
    private PlayerController playerController; // Reference to the PlayerController script
    public GameObject upgradePanelUiGameObject; // Reference to the upgrade panel UI GameObject

    private Bullet bulletCS;
    [Header("Bullet Upgrade Settings")]
    public float bulletUpgradeRate = 10f; // Cost to upgrade the bullet
    public TextMeshProUGUI bulletDamageText;
    public TextMeshProUGUI bulletRangeText;

    // Header("Heal Upgrade Settings")]
    private float healUpgradeRate = 100f; // Cost to upgrade the heal

    [Header("Cricle Upgrade Settings")]
    public float circleUpgradeRate = 1f; // Cost to upgrade the circle
    public TextMeshProUGUI circleDamageText;
    public TextMeshProUGUI circleRadiusText;

    void Awake()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>(); 
        bulletCS = GameObject.FindFirstObjectByType<Bullet>(); 
    }
    void Start()
    {
        upgradePanelUiGameObject.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    public void BulletUpgrade()
    {
        bulletCS.damage += bulletUpgradeRate;
        DisableUIPanel();


    }
    public void HealUpgrade()
    {
        playerController.UpdateHealth(healUpgradeRate);
        DisableUIPanel();
    }
    public void EnableUIPanel()
    {
        Time.timeScale = 0f; // Pause the game
        upgradePanelUiGameObject.SetActive(true); // Activate the upgrade panel UI
        UpdateBulletUI(); // Update the bullet UI text  
        UpdateCircleUI(); // Update the circle UI text
    }
    public void DisableUIPanel()
    {
          Time.timeScale = 1f; // Resume the game
          upgradePanelUiGameObject.SetActive(false); // Deactivate the upgrade panel UI
    }
    private void UpdateBulletUI()
    {
        bulletDamageText.text = "Bullet Damage: " + bulletCS.damage.ToString("F1");
        bulletRangeText.text = "Bullet Range: " + playerController.attackRange.ToString("F1"); // Assuming lifetime is the range
    }
    private void UpdateCircleUI()
    {
       
    }
}
