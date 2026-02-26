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
    public int xp;
    public int soul;

    public enum CombatType { Melee, Ranged }

    [Header("Combat Type")]
    public CombatType combatType = CombatType.Melee;
    private bool isAttacking;

    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float rangedAttackRange = 5f;
    public float rangedAttackCooldown = 2f;
    private float lastRangedAttackTime;

    [Header("Melee Attack Settings")]
    public float meleeAttackRange = 1.5f;
    public float meleeAttackCooldown = 1f;
    public float meleeDamage = 10f;
    private float lastMeleeAttackTime;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float stopDistance = 1f;

    private Transform player;
    private CharacterMovement playerScript;

    void Start()
    {
        erb = GetComponent<Rigidbody2D>();
        ea = GetComponent<Animator>();
        esr = GetComponent<SpriteRenderer>();
        eas = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerScript = playerObj.GetComponent<CharacterMovement>();
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (playerScript != null && !playerScript.IsAlive())
        {
            StopAllActions();
            return;
        }
        AnimatorStateInfo state = ea.GetCurrentAnimatorStateInfo(0);

        if (!state.IsName("Attack"))
        {
            isAttacking = false;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (player.position.x < transform.position.x)
        {
            esr.flipX = true;
        }
        else
        {
            esr.flipX = false;
        }

        if (combatType == CombatType.Ranged)
        {
            HandleRangedCombat(distanceToPlayer);
        }
        else if (combatType == CombatType.Melee)
        {
            HandleMeleeCombat(distanceToPlayer);
        }

        if (ea != null)
        {
            float currentSpeed = Mathf.Abs(erb.linearVelocity.x);
            ea.SetFloat("Speed", currentSpeed);
        }
    }
    void StopAllActions()
    {
        isAttacking = false;

        if (ea != null)
        {
            ea.ResetTrigger("Attack");
        }

        erb.linearVelocity = Vector2.zero;
    }
    void HandleRangedCombat(float distance)
    {
        if (distance <= rangedAttackRange &&
            Time.time >= lastRangedAttackTime + rangedAttackCooldown &&
            !isAttacking)
        {
            StartRangedAttack();
            lastRangedAttackTime = Time.time;
        }
        else if (distance > rangedAttackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            erb.linearVelocity = new Vector2(0, erb.linearVelocity.y);
        }
    }

    void HandleMeleeCombat(float distance)
    {
        if (distance <= meleeAttackRange && Time.time >= lastMeleeAttackTime + meleeAttackCooldown)
        {
            MeleeAttack();
            lastMeleeAttackTime = Time.time;
        }
        else if (distance > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            erb.linearVelocity = new Vector2(0, erb.linearVelocity.y);
        }
    }

    void StartRangedAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        if (ea != null)
            ea.SetTrigger("Attack");

        erb.linearVelocity = new Vector2(0, erb.linearVelocity.y);
    }
    public void EndAttack()
    {
        isAttacking = false;
    }
    public void SpawnProjectile()
    {
        if (isDead) return;

        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        if (playerScript != null && !playerScript.IsAlive())
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity);

        if (eas != null && attaackSound != null)
            eas.PlayOneShot(attaackSound);

        Vector2 direction = (player.position - firePoint.position).normalized;

        Enemy1Fire fireball = projectile.GetComponent<Enemy1Fire>();
        if (fireball != null)
            fireball.direction = direction;
    }
    void MeleeAttack()
    {
        if (ea != null)
            ea.SetTrigger("Attack");

        if (eas != null && attaackSound != null)
            eas.PlayOneShot(attaackSound);

        CharacterMovement playerScript = player.GetComponent<CharacterMovement>();
        if (playerScript != null)
        {
            playerScript.GetDamage(meleeDamage);
        }

        erb.linearVelocity = new Vector2(0, erb.linearVelocity.y);
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        erb.linearVelocity = new Vector2(direction.x * moveSpeed, erb.linearVelocity.y);
    }

    public void GetDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        isAttacking = false;

        if (ea != null)
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

        isAttacking = false;

        if (ea != null)
            ea.SetTrigger("Die");
        if (eas != null && dieSound != null)
            eas.PlayOneShot(dieSound);

        CharacterMovement player = Object.FindFirstObjectByType<CharacterMovement>();
        if (player != null)
        {
            player.EnemyDied();
            player.GetXP(xp);
            player.CollectSoul(soul);
        }

        Destroy(gameObject, 0.9f);
    }

    private void OnDrawGizmosSelected()
    {
        if (combatType == CombatType.Ranged)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
        }
        else if (combatType == CombatType.Melee)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
        }
    }
}