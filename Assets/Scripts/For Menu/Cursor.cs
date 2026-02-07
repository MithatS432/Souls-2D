using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    public AudioClip cursorSound;
    private AudioSource audioSource;

    public GameObject worldClickEffect;
    public Camera gameCamera;

    void Start()
    {
        UnityEngine.Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;

        if (gameCamera == null)
            gameCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
            PlayWorldEffect();
        }
    }

    void PlayClickSound()
    {
        if (SettingsManager.Instance == null || !SettingsManager.Instance.SoundFXEnabled)
            return;

        if (cursorSound != null)
            audioSource.PlayOneShot(cursorSound);
    }

    void PlayWorldEffect()
    {
        if (SettingsManager.Instance == null || !SettingsManager.Instance.VisualFXEnabled)
            return;

        if (worldClickEffect == null || gameCamera == null)
            return;

        Vector3 mousePos = Input.mousePosition;

        float sens = SettingsManager.Instance.MouseSensitivity;

        mousePos += new Vector3(
            Input.GetAxis("Mouse X") * sens * 50f,
            Input.GetAxis("Mouse Y") * sens * 50f,
            0f
        );

        mousePos.z = 10f;

        Vector3 worldPos = gameCamera.ScreenToWorldPoint(mousePos);

        GameObject effect = Instantiate(worldClickEffect, worldPos, Quaternion.identity);
        Destroy(effect, 1f);
    }

}
