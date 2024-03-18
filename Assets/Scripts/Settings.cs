using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 
///		Attach to the SettingsManager in the Main Scene heirarchy.
///		Handles adjustment of audio settings in the Settings Menu.
///		Uses the SaveSystem to save/load settings.
/// 
/// </summary>

public class Settings : MonoBehaviour
{
	// is a singleton
	public static Settings Instance;

	// UI objects to be assigned in the editor
	[Header("Audio")]
	[SerializeField] private AudioMixer volumeMixer;
	[SerializeField] private Slider masterVolumeSlider;
	[SerializeField] private Slider sfxVolumeSlider;
	[SerializeField] private Slider musicVolumeSlider;
	[SerializeField] private AudioSource[] musicTracks;
	[SerializeField] private AudioSource menuButtonClick;
	[SerializeField] private AudioSource backButtonClick;

	[Header("Text Fields")]
	[SerializeField] private TextMeshProUGUI musicGenreText;

	// \/  data that gets saved|loaded  \/

	private float masterVolumeSliderValue;
	private float sfxVolumeSliderValue;
	private float musicVolumeSliderValue;
	private int currentMusicIndex;

	// /\  data that gets saved|loaded  /\

	// default settings
	private float defaultMasterVol = 0.25f;
	private float defaultSFXVol = 1f;
	private float defaultMusicVol = 1f;
	private int defaultTrackIndex = 0;

	// cache data
	private SettingsSaveDataModel saveData;

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
		// try to load saved settings
		saveData = SaveSystem.Instance.LoadSettingsData();

		if(saveData != null) // apply loaded settings
		{
			Debug.Log("[ SETTINGS ] Applying saved settings...");
			ApplySaveData(saveData);
		}
		else // use defaults
		{
			Debug.Log("[ SETTINGS ] Applying default settings...");
			UseDefaultSettings();
		}
	}

	/// <summary>
	/// 
	///		Assigned to an event on the Back button in the Settings menu to auto-save settings.
	/// 
	/// </summary>
	public void SaveDataToFile()
	{

		saveData = new SettingsSaveDataModel(masterVolumeSliderValue,
							sfxVolumeSliderValue,
							musicVolumeSliderValue,
							currentMusicIndex);

		SaveSystem.Instance.SaveSettingsToFile(saveData);
	}

	/// <summary>
	/// 
	///		Helper method to apply loaded setitngs to the game
	/// 
	/// </summary>
	/// <param name="saveData"></param>
	private void ApplySaveData(SettingsSaveDataModel saveData)
	{

		masterVolumeSliderValue = saveData.MasterVolume;
		sfxVolumeSliderValue = saveData.SfxVolume;
		musicVolumeSliderValue = saveData.MusicVolume;
		currentMusicIndex = saveData.MusicIndex;

		InitializeSettings();
	}

	/// <summary>
	/// 
	///		Helper method for loading default 
	/// 
	/// </summary>
	private void UseDefaultSettings()
	{
		// Default Settings
		masterVolumeSliderValue = defaultMasterVol;
		musicVolumeSliderValue = defaultSFXVol;
		sfxVolumeSliderValue = defaultMusicVol;
		currentMusicIndex = defaultTrackIndex;

		InitializeSettings();
	}

	/// <summary>
	/// 
	///		Helper method for matching Settings menu sliders to Audio Mixer values on startup 
	/// 
	/// </summary>
	private void InitializeSettings()
	{
		// set volume sliders in settings panel
		masterVolumeSlider.value = masterVolumeSliderValue;
		sfxVolumeSlider.value = sfxVolumeSliderValue;
		musicVolumeSlider.value = musicVolumeSliderValue;

		// set volumes in the audio mixer
		SetMasterVolume(masterVolumeSliderValue);
		SetSFXVolume(sfxVolumeSliderValue);
		SetMusicVolume(musicVolumeSliderValue);

		// other settings
		SetMusicTrack(0);
	}

	/// <summary>
	/// 
	///		Set the master volume for the game
	/// 
	/// </summary>
	/// <param name="value"></param>
	public void SetMasterVolume(float value)
	{
		volumeMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20); // magic number 20 provided by SpeedTutor on YouTube
		masterVolumeSliderValue = value;
	}

	/// <summary>
	///	
	///		Set the volume of the game's sound effects (SFX)
	/// 
	/// </summary>
	/// <param name="value"></param>
	public void SetSFXVolume(float value)
	{
		volumeMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20); // magic number 20 provided by SpeedTutor on YouTube
		sfxVolumeSliderValue = value;
	}

	/// <summary>
	/// 
	///		Set the volume of the game's music.
	/// 
	/// </summary>
	/// <param name="value"></param>
	public void SetMusicVolume(float value)
	{
		volumeMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); // magic number 20 provided by SpeedTutor on YouTube
		musicVolumeSliderValue = value;
	}

	/// <summary>
	/// 
	///		Set the music track. Loops from one end of the track list to the other.
	/// 
	/// </summary>
	/// <param name="increment"></param>
	public void SetMusicTrack(int increment)
	{
		currentMusicIndex += increment;
		if (currentMusicIndex >= musicTracks.Length)
		{
			currentMusicIndex = 0;
		}
		if(currentMusicIndex < 0)
		{
			currentMusicIndex = musicTracks.Length - 1;
		}

		foreach (AudioSource track in musicTracks)
		{
			track.Stop();
		}

		musicTracks[currentMusicIndex].Play();
		musicGenreText.text = musicTracks[currentMusicIndex].name;
	}

    /// <summary>
    /// 
    ///		Called from Unity Events when a button is clicked in a menu.
    /// 
    /// </summary>
    public void PlayButtonClick()
	{
		menuButtonClick.Play();
	}

    /// <summary>
    /// 
    ///		Called from Unity Events when a button is clicked in a menu.
    /// 
    /// </summary>
    public void PlayBackButton()
	{
		backButtonClick.Play();
	}
}
