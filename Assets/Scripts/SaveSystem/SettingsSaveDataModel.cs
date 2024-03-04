public class SettingsSaveDataModel
{
	public float MasterVolume { get; private set; }
	public float MusicVolume { get; private set; }
	public float SfxVolume { get; private set; }
	public int MusicIndex { get; private set; }

	public SettingsSaveDataModel(float masterVol, float musicVol, float sfxVol, int musicIndex)
	{
		MasterVolume = masterVol;
		MusicVolume = musicVol;
		SfxVolume = sfxVol;
		MusicIndex = musicIndex;
	}
}
