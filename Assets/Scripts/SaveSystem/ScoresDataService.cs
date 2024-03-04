using System;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoresDataService : MonoBehaviour
{
    public static ScoresDataService Instance;

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
        if(!File.Exists(Application.persistentDataPath + scoresFilepath))
        {
            WriteData(new ScoresSaveDataModel());
        }
    }

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

    private int GetScoreByLevel(int level)
    {
        ScoresSaveDataModel data = LoadData();

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
