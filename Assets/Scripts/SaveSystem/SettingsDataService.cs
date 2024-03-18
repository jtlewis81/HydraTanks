using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 
///		Handles reading/writing to a save file for the game's audio settings
///		The data conforms to the SettingsSaveDataModel class
/// 
/// </summary>

public class SettingsDataService : MonoBehaviour
{
	// the service is a singleton
	public static SettingsDataService Instance;

	// path to the save file
    private string settingsFilepath = "/settings.txt";

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

    /// <summary>
    /// 
    ///		Write data to the save file.
    ///		Uses error catching to avoid program crashing.
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool WriteData(SettingsSaveDataModel data)
	{
		string path = Application.persistentDataPath + settingsFilepath;

		try
		{
			if (File.Exists(path)) // delete old save
			{
				Debug.Log("[ SETTINGS DATA SERVICE ] Overwriting existing data...");
				File.Delete(path);
			}
			else // first save
			{
				Debug.Log("[ SETTINGS DATA SERVICE ] Writing first save file...");
			}

			// write data
			using FileStream stream = File.Create(path);
			stream.Close();

			File.WriteAllText( path,
							data.MasterVolume + "|" +
							data.SfxVolume + "|" +
							data.MusicVolume + "|" +
							data.MusicIndex );

			return true;
		}
		catch (Exception e)
		{
			Debug.LogError("[ SETTINGS DATA SERVICE ] Failed to save data. ERROR: " + e.Message);
			return false;
		}
	}

    /// <summary>
    /// 
    ///		Read data from the save file.
    ///		Uses error catching to avoid program crashing.
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public SettingsSaveDataModel LoadData()
	{
		string path = Application.persistentDataPath + settingsFilepath;
		
		if (!File.Exists(path))
		{
			Debug.LogError("[ SETTINGS DATA SERVICE ] Failure to load. File does not exist@ " + path);
			throw new FileNotFoundException(path + " does not exist!");
		}

		// catch any errors that result from trying to get the save file or parse the data
		try
		{
			string[] rawData = File.ReadAllText(path).Split("|");
			SettingsSaveDataModel data = new SettingsSaveDataModel(float.Parse(rawData[0]),
									float.Parse(rawData[1]),
									float.Parse(rawData[2]),
									int.Parse(rawData[3]));
			return data;
		}
		catch (Exception e)
		{
			Debug.LogError("[ SETTINGS DATA SERVICE ] Failed to load data. ERROR: " + e.Message + e.StackTrace);
			throw e;
		}
	}
}
