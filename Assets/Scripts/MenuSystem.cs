using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
///     Manages accessing various menu screens and loading/unloading levels.
/// 
/// </summary>

public class MenuSystem : MonoBehaviour
{
    // is a singleton
    public static MenuSystem Instance;

    [Header("Menu Objects")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelectMenu;
    [SerializeField] private GameObject[] levelSelectButtons;
    [SerializeField] private GameObject levelLoadingScreen;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TextMeshProUGUI finalScoreTMP;
    [SerializeField] private GameObject highScoreScreen;

    [Header("Default Selected Buttons")]
    [SerializeField] private Button mainMenuLevelSelectButton;
    [SerializeField] private Button mainMenuSettingsButton;
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private Button gameOverRestartButton;

    // for marking a level as selected in the level select menu
    [Header("Level Select Labels")]
    [SerializeField] private TextMeshProUGUI[] levelLabels;
    private Color32 normalLevelLabelColor = new Color32(51, 51, 51, 255);
    private Color32 selectedLevelLabelColor = new Color32(255, 255, 255, 255);

    public int Score { get; set; } // modified in LevelManger of the currently level

    private string selectedLevel = "";

    public bool IsPaused {  get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    ///     Marks the selected level in the Level Select menu.
    /// 
    /// </summary>
    /// <param name="levelName"></param>
    public void SetSelectedLevel(string levelName)
    {
        selectedLevel = levelName;

        switch (levelName)
        {
            case "" :
                {
                    UnsetLevel();
                    break;
                }
            case "Level 1" :
                {
                    UnsetLevel();
                    levelLabels[0].color = selectedLevelLabelColor;
                    break;
                }
            case "Level 2":
                {
                    UnsetLevel();
                    levelLabels[1].color = selectedLevelLabelColor;
                    break;
                }
            case "Level 3":
                {
                    UnsetLevel();
                    levelLabels[2].color = selectedLevelLabelColor;
                    break;
                }
            case "Level 4":
                {
                    UnsetLevel();
                    levelLabels[3].color = selectedLevelLabelColor;
                    break;
                }
            case "Level 5":
                {
                    UnsetLevel();
                    levelLabels[4].color = selectedLevelLabelColor;
                    break;
                }
            default :
                {
                    UnsetLevel();
                    break;
                }
        }
    }

    /// <summary>
    /// 
    ///     Gets called by the start button in the Level Select menu.
    /// 
    /// </summary>
    public void LoadLevel()
    {
        if (string.IsNullOrEmpty(selectedLevel))
        {
            return;
        }
        levelLoadingScreen.SetActive(true);
        levelSelectMenu.SetActive(false);
        mainMenuCanvas.SetActive(false);
        Time.timeScale = 1;
        Invoke("disableLoadingScreen", 1f);
        SceneManager.LoadSceneAsync(selectedLevel, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 
    ///     Pauses the game.
    ///     Opens the Pause Menu when the player activates the Pause input. 
    /// 
    /// </summary>
    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        pauseResumeButton.Select();
    }

    /// <summary>
    /// 
    ///     Unpauses the game when the player clicks the Resume button in the Pause menu.
    /// 
    /// </summary>
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1;
    }
    
    /// <summary>
    /// 
    ///     Exits a level and returns to the Main Menu when the player clicks the Quit button on the Pause menu or Game Over screen.
    ///     Does not submit a score for the level when quitting from Pause.
    /// 
    /// </summary>
    public void QuitToMainMenu()
    {
        Time.timeScale = 0;
        IsPaused = false;
        SceneManager.UnloadSceneAsync(selectedLevel);
        SetSelectedLevel("");
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        mainMenuCanvas.SetActive(true);
        mainMenu.SetActive(true);
        mainMenuLevelSelectButton.Select();
    }

    /// <summary>
    /// 
    ///     Exits the game completely.
    ///     Returns the user to their PC's Desktop or File Explorer window.
    /// 
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 
    ///     Called from the LevelManager.
    ///     Turns the game over screen on, displaying the final score for the current level.
    /// 
    /// </summary>
    public void GameOver()
    {
        Time.timeScale = 0;
        IsPaused = true;
        finalScoreTMP.text = "Final Score: " + Score.ToString();
        gameOverMenu.SetActive(true);
        gameOverRestartButton.Select();
    }

    /// <summary>
    /// 
    ///     Called via a Unity Event on the GameOver screen's Try Again button.
    ///     Restarts the current level without returning to the Main Menu.
    /// 
    /// </summary>
    public void RestartLevel()
    {
        levelLoadingScreen.SetActive(true);
        gameOverMenu.SetActive(false);
        SceneManager.UnloadSceneAsync(selectedLevel);
        Time.timeScale = 1;
        IsPaused = false;
        Invoke("disableLoadingScreen", 1f);
        SceneManager.LoadSceneAsync(selectedLevel, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 
    ///     Turn off the loading screen.
    ///     Invoked after a 1 second delay, enough time for the level to load in.
    /// 
    /// </summary>
    private void disableLoadingScreen()
    {
        levelLoadingScreen.SetActive(false);
    }

    /// <summary>
    /// 
    ///     Helper method used in SetSelectedLevel().
    ///     Resets the level labels' color to normal before the newly selected level's label is marked as selected.
    /// 
    /// </summary>
    private void UnsetLevel()
    {
        foreach(var label in levelLabels)
        {
            label.color = normalLevelLabelColor;
        }
    }

    /// <summary>
    /// 
    ///     Handle which button is selected by default for a gamepad after the Settings menu is closed.
    ///     Depends on if Settings was opened from the Main Menu or the Pause Menu.
    /// 
    /// </summary>
    public void SelectButtonOnExitSettings()
    {
        if (IsPaused)
        {
            pauseMenu.SetActive(true);
            pauseResumeButton.Select();
        }
        else
        {
            mainMenu.SetActive(true);
            mainMenuSettingsButton.Select();
        }
    }
    
}
