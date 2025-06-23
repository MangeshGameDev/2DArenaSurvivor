using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuUIManager : MonoBehaviour
{
    public void StartGame()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting");
    }
}
