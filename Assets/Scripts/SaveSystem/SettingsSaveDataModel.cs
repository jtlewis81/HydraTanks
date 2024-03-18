/// <summary>
/// 
///		Data Model for saving audio settings
/// 
/// </summary>
public class SettingsSaveDataModel
{
	public float MasterVolume { get; private set; }
	public float SfxVolume { get; private set; }
	public float MusicVolume { get; private set; }
	public int MusicIndex { get; private set; }

	public SettingsSaveDataModel(float masterVol, float sfxVol, float musicVol, int musicIndex)
	{
		MasterVolume = masterVol;
		SfxVolume = sfxVol;
		MusicVolume = musicVol;
		MusicIndex = musicIndex;
	}
}
