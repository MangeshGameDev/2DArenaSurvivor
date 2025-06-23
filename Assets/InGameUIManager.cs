using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    public GameObject gameOverUIGameObject;
    [Header("In-Game UI Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesLeftText;
    private void Start()
    {
        gameOverUIGameObject.SetActive(false);
    }

    private void Update()
    {
        ShowGameOverUI();
        UpdateInGameUI();
    }
    private void ShowGameOverUI()
    {
        if(PlayerController.instance.isDead)
        {
            gameOverUIGameObject.SetActive(true);
        }
    }
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnExitButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1); 
    }

    private void UpdateInGameUI()
    {
        waveText.text = "Wave: " + WaveSystem.Instance.waveNumber.ToString();
        enemiesLeftText.text = "Enemies Left: " + WaveSystem.Instance.enemyCount.ToString();
    }

}

