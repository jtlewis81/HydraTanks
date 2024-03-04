using System;
using System.IO;
using UnityEngine;

public class SettingsDataService : MonoBehaviour
{
	public static SettingsDataService Instance;

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
							data.MusicVolume + "|" +
							data.SfxVolume + "|" +
							data.MusicIndex );

			return true;
		}
		catch (Exception e)
		{
			Debug.LogError("[ SETTINGS DATA SERVICE ] Failed to save data. ERROR: " + e.Message);
			return false;
		}
	}

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
