
using System.Security.Cryptography;
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
    [SerializeField] private GameObject levelSelectionIndicator;
    [SerializeField] private GameObject[] levelSelectButtons;

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
                    levelSelectionIndicator.SetActive(false);
                    break;
                }
            case "Level 1" :
                {
                    levelSelectionIndicator.SetActive(true);
                    levelSelectionIndicator.transform.position = levelSelectButtons[0].transform.position;
                    break;
                }
            case "Level 2":
                {
                    levelSelectionIndicator.SetActive(true);
                    levelSelectionIndicator.transform.position = levelSelectButtons[1].transform.position;
                    break;
                }
            case "Level 3":
                {
                    levelSelectionIndicator.SetActive(true);
                    levelSelectionIndicator.transform.position = levelSelectButtons[2].transform.position;
                    break;
                }
            case "Level 4":
                {
                    levelSelectionIndicator.SetActive(true);
                    levelSelectionIndicator.transform.position = levelSelectButtons[3].transform.position;
                    break;
                }
            case "Level 5":
                {
                    levelSelectionIndicator.SetActive(true);
                    levelSelectionIndicator.transform.position = levelSelectButtons[4].transform.position;
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

    private void disableLoadingScreen()
    {
        levelLoadingScreen.SetActive(false);
    }

}
