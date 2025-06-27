using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UpgradeManager : MonoBehaviour
{
    // dependencies
    public static UpgradeManager instance; // Singleton instance
    public PlayerController playerController; // Reference to the PlayerController script
    public GameObject upgradePanelUiGameObject; // Reference to the upgrade panel UI GameObject

   
    [Header("Bullet Upgrade Settings")]
    public float bulletDamage = 50f; // Initial bullet damage
    public float bulletDamageUpgradeRate = 5f; // Cost to upgrade the bullet
    public float bulletRangeUpgradeRate = 0.25f; // Cost to upgrade the bullet range
    public TextMeshProUGUI bulletDamageText;
    public TextMeshProUGUI bulletRangeText;

    

    [Header("Cricle Upgrade Settings")]
    public CircleAttack circleAttack; // Reference to the CircleAttack component
    public float circleDamageUpgradeRate = 2f; // Cost to upgrade the circle
    public float circleRangeUpgradeRate = 0.25f; // Cost to upgrade the circle attack range
    public TextMeshProUGUI circleDamageText;
    public TextMeshProUGUI circleRadiusText;
    

    void Awake()
    {

        if(instance == null)
        {
            instance = this; // Set the singleton instance
        }

       
    }
    void Start()
    {
        upgradePanelUiGameObject.SetActive(false); 
    }
    public void BulletUpgrade()
    {
        bulletDamage += bulletDamageUpgradeRate;
        bulletDamage = Mathf.Clamp(bulletDamage, 5f, 150f); // Ensure bullet damage does not exceed a maximum value
        playerController.attackRange += bulletRangeUpgradeRate ; // Assuming attackRange is the range of the bullets
        playerController.attackRange = Mathf.Clamp(playerController.attackRange, 1f, 8f); // Ensure attack range does not exceed a maximum value
        DisableUIPanel();
        updatePlayerExpLevel(); // Update the player's experience level after upgrading

    }
    public void HealUpgrade()
    {
        float healAmount = Random.Range(30,70); // Amount to heal
        playerController.UpdateHealth(healAmount);
        DisableUIPanel();
        playerController.UpdateMaxExp(); // Update the player's experience level after healing
    }
    public void CircleUpgrade()
    {
        if (circleAttack != null)
        {
            circleAttack.IncreaseVisualScale(circleRangeUpgradeRate); 
            circleAttack.IncreaseAttackPower(circleDamageUpgradeRate); // Increase the actual attack range
            circleAttack.attackPower = Mathf.Clamp(circleAttack.attackPower + circleDamageUpgradeRate, 1f, 35);
        }
        DisableUIPanel();
        updatePlayerExpLevel(); // Update the player's experience level after upgrading
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
       
        bulletDamageText.text = "Bullet Damage: " + bulletDamage.ToString();
        bulletRangeText.text = "Bullet Range: " + playerController.attackRange.ToString(); // Assuming lifetime is the range
    }
    private void UpdateCircleUI()
    {
       circleDamageText.text = "Damage: " + circleAttack.attackPower.ToString();
       circleRadiusText.text = "Radius: " + circleAttack.transform.localScale.x.ToString(); // Assuming the circle's radius is represented by its scale
    }
    private void updatePlayerExpLevel()
    {
        playerController.UpdateMaxExp();
    }
}
