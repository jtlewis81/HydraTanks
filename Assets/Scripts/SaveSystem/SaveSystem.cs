using System;
using TMPro;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
	public static SaveSystem Instance;

	[SerializeField] private SettingsDataService settingsDataService;
	[SerializeField] private ScoresDataService scoresDataService;
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

	public void SubmitScore(int level, int score)
	{
		scoresDataService.SubmitNewScore(level, score);
	}

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
