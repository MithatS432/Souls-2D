using UnityEngine;

public class FollowUIImage : MonoBehaviour
{
    public RectTransform uiElement;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 screenPos = uiElement.position;
        screenPos.z = 10f;

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.y -= 0.8f;

        transform.position = worldPos;
    }
}