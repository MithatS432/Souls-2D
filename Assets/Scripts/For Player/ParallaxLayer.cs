using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float horizontalMultiplier = 0.5f;

    [Range(0f, 1f)]
    public float verticalMultiplier = 0.1f;

    [SerializeField] private bool lockY = true;

    private Transform cam;
    private Vector3 lastCamPos;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        float xMove = delta.x * horizontalMultiplier;
        float yMove = lockY ? 0f : delta.y * verticalMultiplier;

        transform.position += new Vector3(xMove, yMove, 0f);

        lastCamPos = cam.position;
    }
}