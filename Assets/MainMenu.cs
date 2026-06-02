using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string cutsceneSceneName = "Cutscene1";

    public void PlayGame()
    {
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        Debug.Log("Loading scene index: " + nextSceneIndex);

        SceneManager.LoadScene(nextSceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}