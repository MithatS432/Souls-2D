using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button aboutGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    public GameObject settingsPanel;
    public GameObject aboutGamePanel;

    [SerializeField] private CanvasGroup fadePanel;

    private float fadeDuration = 0.6f;

    private bool isTransitioning = false;

    [SerializeField] private Toggle masterSoundToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle visualToggle;

    [SerializeField] private Slider dpiSlider;



    void Start()
    {
        fadePanel.alpha = 0;
        fadePanel.gameObject.SetActive(false);

        startButton.onClick.AddListener(() => StartCoroutine(StartGame()));
        settingsButton.onClick.AddListener(() => StartCoroutine(OpenSettings()));
        aboutGameButton.onClick.AddListener(() => StartCoroutine(OpenAboutGame()));
        quitButton.onClick.AddListener(() => StartCoroutine(QuitGameRoutine()));
        backButton.onClick.AddListener(() => StartCoroutine(BackToMenu()));

        dpiSlider.onValueChanged.RemoveAllListeners();

        dpiSlider.value = SettingsManager.Instance.MouseSensitivity;

        dpiSlider.onValueChanged.AddListener(OnDPISliderChanged);

        SetupToggles();
    }
    void OnDPISliderChanged(float value)
    {
        SettingsManager.Instance.SetMouseSensitivity(value);
    }

    void SetupToggles()
    {
        masterSoundToggle.onValueChanged.RemoveAllListeners();
        soundToggle.onValueChanged.RemoveAllListeners();
        visualToggle.onValueChanged.RemoveAllListeners();

        masterSoundToggle.isOn = SettingsManager.Instance.MasterSoundEnabled;
        soundToggle.isOn = SettingsManager.Instance.SoundFXEnabled;
        visualToggle.isOn = SettingsManager.Instance.VisualFXEnabled;

        masterSoundToggle.onValueChanged.AddListener(OnMasterSoundChanged);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        visualToggle.onValueChanged.AddListener(OnVisualToggleChanged);
    }

    void OnMasterSoundChanged(bool value)
    {
        SettingsManager.Instance.SetMasterSound(value);
    }

    void OnSoundToggleChanged(bool value)
    {
        SettingsManager.Instance.SetSoundFX(value);
    }

    void OnVisualToggleChanged(bool value)
    {
        SettingsManager.Instance.SetVisualFX(value);
    }



    #region Scene Start
    IEnumerator StartGame()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        fadePanel.gameObject.SetActive(true);
        yield return FadeCanvas(fadePanel, 0, 1);

        SceneManager.LoadScene("My World");
    }
    #endregion

    #region Panels

    IEnumerator OpenSettings()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        SetupToggles();

        backButton.gameObject.SetActive(true);

        yield return FadeOutPanel(aboutGamePanel);

        settingsPanel.SetActive(true);
        yield return FadeInPanel(settingsPanel);

        ToggleMainButtons(false);

        isTransitioning = false;
    }


    IEnumerator OpenAboutGame()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        backButton.gameObject.SetActive(true);

        yield return FadeOutPanel(settingsPanel);

        aboutGamePanel.SetActive(true);
        yield return FadeInPanel(aboutGamePanel);

        ToggleMainButtons(false);

        isTransitioning = false;
    }


    IEnumerator BackToMenu()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        backButton.gameObject.SetActive(false);
        ToggleMainButtons(true);

        yield return FadeOutPanel(settingsPanel);
        yield return FadeOutPanel(aboutGamePanel);

        isTransitioning = false;
    }



    void ToggleMainButtons(bool state)
    {
        startButton.gameObject.SetActive(state);
        settingsButton.gameObject.SetActive(state);
        aboutGameButton.gameObject.SetActive(state);
        quitButton.gameObject.SetActive(state);
    }

    #endregion

    #region Fade Helpers

    IEnumerator FadeInPanel(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        cg.alpha = 0;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = EaseInOut(t / fadeDuration);
            yield return null;
        }

        cg.alpha = 1;
    }

    IEnumerator FadeOutPanel(GameObject panel)
    {
        if (!panel.activeSelf) yield break;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        float start = cg.alpha;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, 0, EaseInOut(t / fadeDuration));
            yield return null;
        }

        cg.alpha = 0;
        panel.SetActive(false);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float start, float end)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, EaseInOut(t / fadeDuration));
            yield return null;
        }

        cg.alpha = end;
    }

    float EaseInOut(float x)
    {
        return x * x * (3f - 2f * x);
    }

    #endregion

    IEnumerator QuitGameRoutine()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        ToggleMainButtons(false);

        fadePanel.gameObject.SetActive(true);

        yield return FadeCanvas(fadePanel, 0, 1);

        yield return new WaitForSeconds(0.4f);

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
