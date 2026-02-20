using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



public class CharacterMovement : MonoBehaviour
{
    private enum GameState
    {
        Gameplay,
        Paused,
        SkillTree,
        Transition
    }

    private GameState currentState = GameState.Gameplay;


    [Header("Component References")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource movementSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Passing References")]
    public Transform templeArea;
    public Transform forestArea;
    public Transform kingdomArea;
    public Transform frozenArea;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Music Manager")]
    [SerializeField] private MusicAmbientManager musicManager;
    public AudioClip skillTreeSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;

    [Header("Game UI")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    public GameObject skillTreeUI;
    private Animator skillTreeAnimator;
    private float skillTreeAnimLength = 1f;
    [SerializeField] private float tabCooldown = 1f;
    private float nextTabAllowedTime = 0f;

    [Header("Player Stats")]
    [SerializeField] private float moveSpeed = 5f;
    public float runSpeed = 10f;
    private float currentSpeed;
    [SerializeField] private float jumpForce = 10f;
    public bool isGrounded;
    private float x;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    [SerializeField] private float wallSlideSpeed = 2f;
    private bool isTouchingWall;

    [SerializeField] private int maxCombo = 2;
    private int currentComboIndex = 0;
    private bool canQueueNextAttack = false;


    [Header("Parallax Roots")]
    [SerializeField] private GameObject forestParallaxRoot;
    [SerializeField] private GameObject kingdomParallaxRoot;
    [SerializeField] private GameObject frozenParallaxRoot;
    [SerializeField] private float backgroundYOffset = -2f;

    void Start()
    {
        skillTreeAnimator = skillTreeUI.GetComponent<Animator>();
        skillTreeUI.SetActive(false);
        currentSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);
        forestParallaxRoot.SetActive(false);
        kingdomParallaxRoot.SetActive(false);
        frozenParallaxRoot.SetActive(false);
    }

    void PauseGame()
    {
        if (currentState != GameState.Gameplay) return;

        currentState = GameState.Paused;

        Time.timeScale = 0f;
        resumeButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
    }

    void ResumeGame()
    {
        if (currentState != GameState.Paused) return;

        currentState = GameState.Gameplay;

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
        if (Input.GetKeyDown(KeyCode.Tab) &&
       Time.unscaledTime >= nextTabAllowedTime)
        {
            nextTabAllowedTime = Time.unscaledTime + tabCooldown;

            if (currentState == GameState.Gameplay)
                OpenSkillTree();
            else if (currentState == GameState.SkillTree)
                CloseSkillTreeImmediate();
        }

        if (currentState != GameState.Gameplay)
            return;

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
            sfxSource.PlayOneShot(jumpSound);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleAttackInput();
        }

        HandleMovementSound();
    }
    void OpenSkillTree()
    {
        currentState = GameState.SkillTree;

        pauseButton.gameObject.SetActive(false);
        skillTreeUI.SetActive(true);
        skillTreeAnimator.Play("InventoryOpen", 0, 0f);
        skillTreeAnimator.SetBool("IsOpen", true);

        Time.timeScale = 0f;
        sfxSource.PlayOneShot(skillTreeSound);
    }

    void CloseSkillTreeImmediate()
    {
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        skillTreeAnimator.SetBool("IsOpen", false);
        sfxSource.PlayOneShot(skillTreeSound);

        yield return new WaitForSecondsRealtime(skillTreeAnimLength);

        skillTreeUI.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        Time.timeScale = 1f;
        currentState = GameState.Gameplay;
    }
    void HandleMovementSound()
    {
        bool isMoving = Mathf.Abs(x) > 0.1f;

        if (currentState != GameState.Gameplay)
        {
            if (movementSource.isPlaying)
                movementSource.Stop();
            return;
        }

        if (isGrounded && isMoving)
        {
            AudioClip targetClip = (currentSpeed == runSpeed) ? runSound : walkSound;

            if (movementSource.clip != targetClip)
            {
                movementSource.clip = targetClip;
                movementSource.pitch = Random.Range(0.95f, 1.05f);
                movementSource.Play();
            }
        }
        else
        {
            if (movementSource.isPlaying)
                movementSource.Stop();
        }
    }


    private void FixedUpdate()
    {
        if (currentState != GameState.Gameplay)
            return;

        isTouchingWall = IsTouchingWall();

        float targetX = x * currentSpeed;

        if (isTouchingWall && !isGrounded)
        {
            targetX = 0f;

            if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(0, -wallSlideSpeed);
                return;
            }
        }

        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    bool IsTouchingWall()
    {
        Vector2 origin = transform.position;
        float distance = 0.6f;

        RaycastHit2D leftHit = Physics2D.Raycast(origin, Vector2.left, distance, LayerMask.GetMask("Wall"));
        RaycastHit2D rightHit = Physics2D.Raycast(origin, Vector2.right, distance, LayerMask.GetMask("Wall"));

        return leftHit.collider != null || rightHit.collider != null;
    }

    void HandleAttackInput()
    {
        if (currentComboIndex == 0)
        {
            currentComboIndex = 1;
            animator.SetInteger("AttackIndex", 1);
        }
        else if (canQueueNextAttack && currentComboIndex < maxCombo)
        {
            currentComboIndex++;
            animator.SetInteger("AttackIndex", currentComboIndex);
            canQueueNextAttack = false;
        }
    }

    public void EnableComboWindow()
    {
        canQueueNextAttack = true;
    }

    public void ResetCombo()
    {
        currentComboIndex = 0;
        canQueueNextAttack = false;
        animator.SetInteger("AttackIndex", 0);
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Forest"))
        {
            StartCoroutine(FadeAndTeleport(forestArea, "Forest"));
        }
        else if (other.CompareTag("Temple"))
        {
            StartCoroutine(FadeAndTeleport(templeArea, "Temple"));
        }
        else if (other.CompareTag("Kingdom"))
        {
            StartCoroutine(FadeAndTeleport(kingdomArea, "Kingdom"));
        }
        else if (other.CompareTag("Frozen"))
        {
            StartCoroutine(FadeAndTeleport(frozenArea, "Frozen"));
        }
    }

    void ActivateParallax(string area)
    {
        forestParallaxRoot.SetActive(area == "Forest");
        kingdomParallaxRoot.SetActive(area == "Kingdom");
        frozenParallaxRoot.SetActive(area == "Frozen");

        ResetParallaxPosition(area);
    }
    void ResetParallaxPosition(string area)
    {
        Vector3 offset = new Vector3(0, backgroundYOffset, 0);

        if (area == "Forest")
            forestParallaxRoot.transform.position = forestArea.position + offset;

        if (area == "Kingdom")
            kingdomParallaxRoot.transform.position = kingdomArea.position + offset;

        if (area == "Frozen")
            frozenParallaxRoot.transform.position = frozenArea.position + offset;
    }

    IEnumerator FadeAndTeleport(Transform targetArea, string areaName)
    {
        if (currentState == GameState.SkillTree)
        {
            skillTreeUI.SetActive(false);
            Time.timeScale = 1f;
            pauseButton.gameObject.SetActive(true);
        }

        currentState = GameState.Transition;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1f));

        transform.position = targetArea.position;
        ActivateParallax(areaName);

        if (musicManager != null)
            musicManager.ChangeAreaMusic(areaName);

        yield return StartCoroutine(Fade(0f));

        fadeImage.gameObject.SetActive(false);

        currentState = GameState.Gameplay;
    }


    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);

            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }

        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }

}
