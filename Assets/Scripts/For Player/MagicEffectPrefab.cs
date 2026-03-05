using UnityEngine;

public class MagicEffectPrefab : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float damage = 30f;
    private float speed = 20f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.GetDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
