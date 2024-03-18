using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 
///		The SaveSystem uses DataServices, calls their methods, and uses error checking
///		  because we're handling file IO, and this can cause the game to crash if the exception is not handled.
///		The individual services handle the files themselves.		
///		The LevelManager in each level calls the scores service methods.
///		The Settings script in the Main Scene calls the settings service mehtods.
/// 
/// </summary>

public class SaveSystem : MonoBehaviour
{
	// the save system is a singleton, as are the data services so there is no duplicate IO calls
	public static SaveSystem Instance;

	// references to data services
	// optionally, these don't have to be serialized since they can be assigned in Start()
	[SerializeField] private SettingsDataService settingsDataService;
	[SerializeField] private ScoresDataService scoresDataService;
    
	// reference to the score text on the high scores menu screen
	[SerializeField] private TextMeshProUGUI scoresText;

    private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// ensure that the services were assigned in the Unity editor
	private void Start()
	{
		if (settingsDataService == null)
		{
            settingsDataService = GetComponent<SettingsDataService>();
        }

		if (scoresDataService == null)
		{
			scoresDataService = GetComponent<ScoresDataService>();
        }
    }

	/// <summary>
	/// 
	///		Calls the SettingsDataService method that writes data to a save file.
	/// 
	/// </summary>
	/// <param name="data"></param>
    public void SaveSettingsToFile(SettingsSaveDataModel data)
	{
		if (settingsDataService.WriteData(data))
		{
			Debug.Log("[ SAVE SYSTEM ] Settings Data saved successfully!");
		}
		else
		{
			Debug.LogError("[ SAVE SYSTEM ] Settings Data failed to save!");
		}
	}

	/// <summary>
	/// 
	///		Calls the SettingsDataService method that reads data from the save file.
	/// 
	/// </summary>
	/// <returns></returns>
	public SettingsSaveDataModel LoadSettingsData()
	{
		try
		{
			SettingsSaveDataModel data = settingsDataService.LoadData();

			Debug.Log("[ SAVE SYSTEM ] Settings Data loaded successfully!");
			return data;
		}
		catch (Exception e)
		{
			Debug.LogError("[ SAVE SYSTEM ] Settings Data failed to load! " + e.Message);
			return null;
		}
	}

	/// <summary>
	/// 
	///		Calls the ScoreDataService method that writes data to a save file.
	/// 
	/// </summary>
	/// <param name="data"></param>
    public void SaveScoresToFile(ScoresSaveDataModel data)
    {
        if (scoresDataService.WriteData(data))
        {
            Debug.Log("[ SAVE SYSTEM ] Scores Data saved successfully!");
        }
        else
        {
            Debug.LogError("[ SAVE SYSTEM ] Scores Data failed to save!");
        }
    }

    /// <summary>
    /// 
    ///		Calls the ScoreDataService method that reads data from the save file.
    /// 
    /// </summary>
    /// <returns></returns>
    public ScoresSaveDataModel LoadScoresData()
    {
        try
        {
            ScoresSaveDataModel data = scoresDataService.LoadData();

            Debug.Log("[ SAVE SYSTEM ] Scores Data loaded successfully!");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("[ SAVE SYSTEM ] Scores Data failed to load! " + e.Message);
            return null;
        }
    }

	/// <summary>
	/// 
	///		Calls the ScoresDataService method that attempts to update the saved high scores.
	/// 
	/// </summary>
	/// <param name="level"></param>
	/// <param name="score"></param>
	public void SubmitScore(int level, int score)
	{
		scoresDataService.SubmitNewScore(level, score);
	}

	/// <summary>
	/// 
	///		Assigned to a UnityEvent in the editor.
	///		Gets called when the High Scores menu is opened.
	/// 
	/// </summary>
    public void RefreshScoreBoard()
    {
        ScoresSaveDataModel data = scoresDataService.LoadData();
        scoresText.text = "Level 1 : " + data.Level1Score + "\n" +
                            "Level 2 : " + data.Level2Score + "\n" +
                            "Level 3 : " + data.Level3Score + "\n" +
                            "Level 4 : " + data.Level4Score + "\n" +
                            "Level 5 : " + data.Level5Score;
    }
}
