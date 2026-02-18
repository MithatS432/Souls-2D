using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class CharacterMovement : MonoBehaviour
{
    [Header("Component References")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [Header("Passing References")]
    public Transform templeArea;
    public Transform forestArea;


    [Header("Game UI")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;


    [Header("Player Stats")]
    public bool isAlive = true;
    [SerializeField] private float moveSpeed = 5f;
    public float runSpeed = 10f;
    private float currentSpeed;
    [SerializeField] private float jumpForce = 10f;
    public bool isGrounded;
    private float x;

    void Start()
    {
        currentSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        resumeButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
    }
    void ResumeGame()
    {
        Time.timeScale = 1f;
        resumeButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }
    void ExitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        if (!isAlive) return;

        x = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(x));

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = moveSpeed;


        if (x < 0) spriteRenderer.flipX = true;
        else if (x > 0) spriteRenderer.flipX = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        float targetX = x * currentSpeed;

        if (IsTouchingWall() && !isGrounded)
            targetX = 0f;

        Vector2 movement = new Vector2(targetX, rb.linearVelocity.y);
        rb.linearVelocity = movement;
    }
    bool IsTouchingWall()
    {
        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, 0.6f, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Forest"))
        {
            transform.position = forestArea.transform.position;
        }
        if (other.gameObject.CompareTag("Temple"))
        {
            transform.position = templeArea.transform.position;
        }
    }
}
