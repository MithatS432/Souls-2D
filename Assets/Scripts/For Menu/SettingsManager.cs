using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public bool SoundFXEnabled { get; private set; } = true;
    public bool VisualFXEnabled { get; private set; } = true;
    public bool MasterSoundEnabled { get; private set; } = true;

    private const string SOUND_KEY = "SoundFX";
    private const string VISUAL_KEY = "VisualFX";
    private const string MASTER_KEY = "MasterSound";

    public float MouseSensitivity { get; private set; } = 1f;

    private const string DPI_KEY = "MouseSensitivity";
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSoundFX(bool state)
    {
        SoundFXEnabled = state;
        PlayerPrefs.SetInt(SOUND_KEY, state ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVisualFX(bool state)
    {
        VisualFXEnabled = state;
        PlayerPrefs.SetInt(VISUAL_KEY, state ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterSound(bool state)
    {
        MasterSoundEnabled = state;
        PlayerPrefs.SetInt(MASTER_KEY, state ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMasterSound();
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey(SOUND_KEY))
            SoundFXEnabled = PlayerPrefs.GetInt(SOUND_KEY) == 1;
        else
            SoundFXEnabled = true;

        if (PlayerPrefs.HasKey(VISUAL_KEY))
            VisualFXEnabled = PlayerPrefs.GetInt(VISUAL_KEY) == 1;
        else
            VisualFXEnabled = true;

        if (PlayerPrefs.HasKey(MASTER_KEY))
            MasterSoundEnabled = PlayerPrefs.GetInt(MASTER_KEY) == 1;
        else
            MasterSoundEnabled = true;

        MouseSensitivity = PlayerPrefs.GetFloat(DPI_KEY, 1f);


        ApplyMasterSound();
    }


    void ApplyMasterSound()
    {
        AudioListener.volume = MasterSoundEnabled ? 1f : 0f;
    }

    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat(DPI_KEY, value);
        PlayerPrefs.Save();
    }
}
