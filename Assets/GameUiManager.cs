using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Panels")]
    public CanvasGroup inventoryPanel;
    public CanvasGroup pausePanel;

    [Header("Keys")]
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Settings")]
    public bool pauseWhenInventoryOpen = true;

    [Header("Debug")]
    public bool inventoryOpen = false;
    public bool paused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameUIManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("GameUIManager is ready.");
    }

    private void Start()
    {
        inventoryOpen = false;
        paused = false;

        HidePanel(inventoryPanel);
        HidePanel(pausePanel);

        UpdateGamePauseState();

        Debug.Log("GameUIManager started. Inventory and pause menu hidden.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            Debug.Log("Inventory key pressed.");
            ToggleInventory();
        }

        if (Input.GetKeyDown(pauseKey))
        {
            Debug.Log("Pause key pressed.");
            TogglePause();
        }
    }

    // =========================
    // INVENTORY
    // =========================

    public void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel is not assigned in GameUIManager!");
            return;
        }

        inventoryOpen = !inventoryOpen;

        if (inventoryOpen)
        {
            ShowPanel(inventoryPanel);
            Debug.Log("Inventory opened.");
        }
        else
        {
            HidePanel(inventoryPanel);
            Debug.Log("Inventory closed.");
        }

        UpdateGamePauseState();
    }

    public void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel is not assigned in GameUIManager!");
            return;
        }

        inventoryOpen = true;
        ShowPanel(inventoryPanel);

        UpdateGamePauseState();

        Debug.Log("Inventory opened.");
    }

    public void CloseInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel is not assigned in GameUIManager!");
            return;
        }

        inventoryOpen = false;
        HidePanel(inventoryPanel);

        UpdateGamePauseState();

        Debug.Log("Inventory closed.");
    }

    // =========================
    // PAUSE MENU
    // =========================

    public void TogglePause()
    {
        if (paused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pausePanel == null)
        {
            Debug.LogError("Pause Panel is not assigned in GameUIManager!");
            return;
        }

        paused = true;
        ShowPanel(pausePanel);

        UpdateGamePauseState();

        Debug.Log("Game paused.");
    }

    public void ResumeGame()
    {
        if (pausePanel == null)
        {
            Debug.LogError("Pause Panel is not assigned in GameUIManager!");
            return;
        }

        paused = false;
        HidePanel(pausePanel);

        UpdateGamePauseState();

        Debug.Log("Game resumed.");
    }

    // =========================
    // GAME PAUSE STATE
    // =========================

    private void UpdateGamePauseState()
    {
        bool shouldPause = false;

        if (paused)
        {
            shouldPause = true;
        }

        if (pauseWhenInventoryOpen && inventoryOpen)
        {
            shouldPause = true;
        }

        Time.timeScale = shouldPause ? 0f : 1f;
    }

    public bool CanPlayerAct()
    {
        if (paused)
            return false;

        if (pauseWhenInventoryOpen && inventoryOpen)
            return false;

        return true;
    }

    // =========================
    // BUTTON FUNCTIONS
    // =========================

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game.");
        Application.Quit();
    }

    // =========================
    // PANEL VISIBILITY
    // =========================

    private void ShowPanel(CanvasGroup group)
    {
        if (group == null)
            return;

        group.gameObject.SetActive(true);

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void HidePanel(CanvasGroup group)
    {
        if (group == null)
            return;

        group.gameObject.SetActive(true);

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}