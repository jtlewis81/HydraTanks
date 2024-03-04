using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public static MenuSystem Instance;

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject levelSelectMenu;
    [SerializeField] private GameObject highScoreScreen;
    [SerializeField] private GameObject levelLoadingScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private GameObject[] levelSelectButtons;

    private TextMeshProUGUI[] levelLabels;
    private Color32 normalLevelLabelColor = new Color32(50, 50, 50, 255);
    private Color32 selectedLevelLabelColor = new Color32(0, 135, 131, 255);

    public int Score { get; set; }

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

    private void Start()
    {
        // not working for some reason ???
        levelLabels = new TextMeshProUGUI[5];
        for(int i = 0; i < levelLabels.Length; i++)
        {
            levelLabels[i] = levelSelectButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        SetSelectedLevel("");
    }

    // gets called by the level buttons in the level select menu
    public void SetSelectedLevel(string levelName)
    {
        selectedLevel = levelName;

        // set selection indicator
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

    // gets called by the start button in the level select menu
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

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 0;
        IsPaused = false;
        SceneManager.UnloadSceneAsync(selectedLevel);
        SetSelectedLevel("");
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        IsPaused = true;
        finalScore.text = "Final Score: " + Score.ToString();
        gameOverMenu.SetActive(true);
    }

    public void RestartLevel()
    {
        levelLoadingScreen.SetActive(true);
        gameOverMenu.SetActive(true);
        SceneManager.UnloadSceneAsync(selectedLevel);
        Time.timeScale = 1;
        IsPaused = false;
        Invoke("disableLoadingScreen", 1f);
        SceneManager.LoadSceneAsync(selectedLevel, LoadSceneMode.Additive);
    }

    private void disableLoadingScreen()
    {
        levelLoadingScreen.SetActive(false);
    }

    private void UnsetLevel()
    {
        foreach(var label in levelLabels)
        {
            label.color = normalLevelLabelColor;
        }
    }
}
