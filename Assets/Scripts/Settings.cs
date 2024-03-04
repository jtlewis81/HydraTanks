using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	public static Settings Instance;

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

		if(saveData != null)
		{
			Debug.Log("[ SETTINGS ] Applying saved settings...");
			ApplySaveData(saveData);

			// indicate that data loaded successfully?
		}
		else
		{
			Debug.Log("[ SETTINGS ] Applying default settings...");
			UseDefaultSettings();
		}
	}

	public void SaveDataToFile()
	{

		saveData = new SettingsSaveDataModel(masterVolumeSliderValue,
							musicVolumeSliderValue,
							sfxVolumeSliderValue,
							currentMusicIndex);

		SaveSystem.Instance.SaveSettingsToFile(saveData);
	}

	private void ApplySaveData(SettingsSaveDataModel saveData)
	{

		masterVolumeSliderValue = saveData.MasterVolume;
		sfxVolumeSliderValue = saveData.SfxVolume;
		musicVolumeSliderValue = saveData.MusicVolume;
		currentMusicIndex = saveData.MusicIndex;

		InitializeSettings();
	}

	private void UseDefaultSettings()
	{
		// Default Settings
		masterVolumeSliderValue = 0.25f;
		sfxVolumeSliderValue = 1f;
		musicVolumeSliderValue = 1f;
		currentMusicIndex = 0;

		InitializeSettings();
	}

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

	public void SetMasterVolume(float value)
	{
		volumeMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20); // magic number 20 provided by SpeedTutor on YouTube
		masterVolumeSliderValue = value;
	}

	public void SetMusicVolume(float value)
	{
		volumeMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); // magic number 20 provided by SpeedTutor on YouTube
		musicVolumeSliderValue = value;
	}

	public void SetSFXVolume(float value)
	{
		volumeMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20); // magic number 20 provided by SpeedTutor on YouTube
		sfxVolumeSliderValue = value;
	}

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

	// Play SFX Clips

	// called from setup in inspector for menus
	public void PlayButtonClick()
	{
		menuButtonClick.Play();
	}
	public void PlayBackButton()
	{
		backButtonClick.Play();
	}
}
