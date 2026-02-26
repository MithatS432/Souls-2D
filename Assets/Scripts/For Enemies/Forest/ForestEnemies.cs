using UnityEngine;

public class ForestEnemies : MonoBehaviour, IDamageable
{
    private Rigidbody2D erb;
    private Animator ea;
    private SpriteRenderer esr;
    private AudioSource eas;


    public AudioClip attaackSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;

    public float health;
    private bool isDead = false;
    public GameObject hurtEffectPrefab;
    void Start()
    {
        erb = GetComponent<Rigidbody2D>();
        ea = GetComponent<Animator>();
        esr = GetComponent<SpriteRenderer>();
        eas = GetComponent<AudioSource>();
    }

    void Update()
    {

    }
    public void GetDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        ea.SetTrigger("Hurt");

        if (hurtEffectPrefab != null)
        {

            Vector3 spawnPos = transform.position + new Vector3(0.5f, 1.5f, 0f);
            GameObject he = Instantiate(hurtEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(he, 1f);
        }
        if (eas != null && hurtSound != null)
            eas.PlayOneShot(hurtSound);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (ea != null)
            ea.SetTrigger("Die");
        if (eas != null && dieSound != null)
            eas.PlayOneShot(dieSound);

        CharacterMovement player = Object.FindFirstObjectByType<CharacterMovement>();
        if (player != null)
        {
            player.EnemyDied();
        }

        Destroy(gameObject, 0.9f);
    }
}
