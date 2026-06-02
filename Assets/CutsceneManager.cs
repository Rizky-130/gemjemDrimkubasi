using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName = "Level1";

    [Header("Cutscene Settings")]
    public bool autoContinue = true;
    public float cutsceneDuration = 8f;

    [Header("Skip Settings")]
    public bool allowSkip = true;
    public KeyCode skipKey = KeyCode.Space;

    private bool hasContinued = false;
    private float timer = 0f;

    private void Start()
    {
        Time.timeScale = 1f;
        timer = 0f;
    }

    private void Update()
    {
        if (hasContinued)
            return;

        if (allowSkip && Input.GetKeyDown(skipKey))
        {
            ContinueToGame();
            return;
        }

        if (autoContinue)
        {
            timer += Time.deltaTime;

            if (timer >= cutsceneDuration)
            {
                ContinueToGame();
            }
        }
    }

    public void ContinueToGame()
    {
        if (hasContinued)
            return;

        hasContinued = true;

        Time.timeScale = 1f;

        Debug.Log("Cutscene finished. Loading: " + nextSceneName);

        SceneManager.LoadScene(nextSceneName);
    }
}