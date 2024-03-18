using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 
///		Handles reading/writing to a save file for the game's high scores
///		The data conforms to the ScoresSaveDataModel class
/// 
/// </summary>

public class ScoresDataService : MonoBehaviour
{
    // the service is a singleton
    public static ScoresDataService Instance;

    // path to the save file
    private string scoresFilepath = "/scores.txt";

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
        // create a new save file with scores of 0 for all levels if no file exists
        // so something shows up in the high scores screen on the first launch
        if(!File.Exists(Application.persistentDataPath + scoresFilepath))
        {
            WriteData(new ScoresSaveDataModel());
        }
    }

    /// <summary>
    /// 
    ///     Write data to the save file.
    ///		Uses error catching to avoid program crashing.
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool WriteData(ScoresSaveDataModel data)
    {
        string path = Application.persistentDataPath + scoresFilepath;

        try
        {
            if (File.Exists(path)) // delete old save
            {
                Debug.Log("[ SCORES DATA SERVICE ] Overwriting existing data...");
                File.Delete(path);
            }
            else // first save
            {
                Debug.Log("[ SCORES DATA SERVICE ] Writing first save file...");
            }

            // write data
            using FileStream stream = File.Create(path);
            stream.Close();

            File.WriteAllText(path, data.Level1Score + "|" +
                                    data.Level2Score + "|" + 
                                    data.Level3Score + "|" + 
                                    data.Level4Score + "|" + 
                                    data.Level5Score + "|");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[ SCORES DATA SERVICE ] Failed to save data. ERROR: " + e.Message);
            return false;
        }
    }

    /// <summary>
    ///
    ///     Read data from the save file.
    ///		Uses error catching to avoid program crashing.
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public ScoresSaveDataModel LoadData()
    {
        string path = Application.persistentDataPath + scoresFilepath;

        // Start() should make sure this exception never happens
        if (!File.Exists(path))
        {
            Debug.LogError("[ SCORES DATA SERVICE ] Failure to load. File does not exist@ " + path);
            throw new FileNotFoundException(path + " does not exist!");
        }

        try
        {
            string[] rawData = File.ReadAllText(path).Split("|");
            ScoresSaveDataModel data = new ScoresSaveDataModel(int.Parse(rawData[0]),
                                                                int.Parse(rawData[1]),
                                                                int.Parse(rawData[2]),
                                                                int.Parse(rawData[3]),
                                                                int.Parse(rawData[4]));

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("[ SCORES DATA SERVICE ] Failed to load data. ERROR: " + e.Message + e.StackTrace);
            throw e;
        }
    }

    /// <summary>
    /// 
    ///     Saves a new high score for the level that just ended.
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <param name="newScore"></param>
    public void SubmitNewScore(int level, int newScore)
    {
        int currentLevelScore = GetScoreByLevel(level);

        if (newScore <= currentLevelScore)
        {
            return;
        }
        else
        {
            ScoresSaveDataModel data = LoadData();

            switch (level)
            {
                case 1:
                    {
                        data.Level1Score = newScore;
                        break;
                    }
                case 2:
                    {
                        data.Level2Score = newScore;
                        break;
                    }
                case 3:
                    {
                        data.Level3Score = newScore;
                        break;
                    }
                case 4:
                    {
                        data.Level4Score = newScore;
                        break;
                    }
                case 5:
                    {
                        data.Level5Score = newScore;
                        break;
                    }
                default: break;
            }

            WriteData(data);
        }
    }

    /// <summary>
    /// 
    ///     Gets the high score for a given level from the save file
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private int GetScoreByLevel(int level)
    {
        // get the save data
        ScoresSaveDataModel data = LoadData();

        // return the score for the level parameter
        switch (level)
        {
            case 1:
                {
                    return data.Level1Score;
                }
            case 2:
                {
                    return data.Level2Score;
                }
            case 3:
                {
                    return data.Level3Score;
                }
            case 4:
                {
                    return data.Level4Score;
                }
            case 5:
                {
                    return data.Level5Score;
                }
            default: return 0;
        }
         
    }
}
