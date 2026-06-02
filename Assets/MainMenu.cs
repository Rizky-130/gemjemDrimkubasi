using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string cutsceneSceneName = "Cutscene1";

    public void PlayGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Play pressed. Loading cutscene: " + cutsceneSceneName);

        SceneManager.LoadScene(cutsceneSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}