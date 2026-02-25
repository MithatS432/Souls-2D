using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private float damage;

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.GetDamage(damage);
        }
    }
}