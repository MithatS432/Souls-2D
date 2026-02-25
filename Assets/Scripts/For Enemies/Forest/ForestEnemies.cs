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
        health -= damage;
        ea.SetTrigger("Hurt");

        if (hurtEffectPrefab != null)
        {
            GameObject he = Instantiate(hurtEffectPrefab, transform.position, Quaternion.identity);
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
        if (ea != null)
            ea.SetTrigger("Die");
        if (dieSound != null)
            eas.PlayOneShot(dieSound);

        Destroy(gameObject, 1.1f);
    }
}
