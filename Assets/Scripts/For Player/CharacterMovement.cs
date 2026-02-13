using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Component References")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [Header("Player Stats")]
    public bool isAlive = true;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    public bool isGrounded;
    private float x;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isAlive) return;

        x = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(x));

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

        Vector2 movement = new Vector2(x * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
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
}
