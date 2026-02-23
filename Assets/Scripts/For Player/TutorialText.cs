using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float showDuration = 2f;

    public GameObject limitCollider;

    void Start()
    {
        StartCoroutine(TutorialFlow());
    }

    IEnumerator TutorialFlow()
    {
        yield return Show("W / A / S / D - Move");
        yield return Show("Left Shift - Run");
        yield return Show("Fire1 - Attack1 and Attack2");
        yield return Show("TAB - Inventory");
        yield return Show("SPACE - Jump");
        yield return Show("SPACE x2 - Double Jump");
        yield return Show("R - Dash");
        yield return Show("F - Magic Power");

        gameObject.SetActive(false);
        limitCollider.SetActive(true);
    }

    IEnumerator Show(string message)
    {
        tutorialText.text = message;

        yield return Fade(1f);

        yield return new WaitForSeconds(showDuration);

        yield return Fade(0f);
    }

    IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}