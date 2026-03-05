using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private CharacterMovement player;

    void Awake()
    {
        player = GetComponentInParent<CharacterMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            float damageToDeal = player.AttackDamage;

            damageable.GetDamage(damageToDeal);
        }
    }
}