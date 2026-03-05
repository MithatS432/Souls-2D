using UnityEngine;

public class Enemy1Fire : MonoBehaviour
{
    Rigidbody2D rb;
    public float damage = 5f;
    public float speed = 10f;
    public Vector2 direction;

    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        if (rb != null && direction != Vector2.zero)
        {
            rb.linearVelocity = direction.normalized * speed;
        }

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovement player = other.GetComponent<CharacterMovement>();
            if (player != null)
            {
                player.GetDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}