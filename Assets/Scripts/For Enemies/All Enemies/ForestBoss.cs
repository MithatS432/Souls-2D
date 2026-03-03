using UnityEngine;

public class ForestBoss : ForestEnemies
{
    [Header("Phase Thresholds (Percentage of Max Health)")]
    [SerializeField, Range(0, 100)] private float phaseTwoHealthPercentage = 90f;
    [SerializeField, Range(0, 100)] private float phaseThreeHealthPercentage = 50f;

    [Header("Phase Damage Settings")]
    [SerializeField] private float phaseTwoMeleeDamage = 30f;
    [SerializeField] private float phaseThreeMeleeDamage = 50f;

    private bool phaseTwoActivated = false;
    private bool phaseThreeActivated = false;
    private float maxHealth;

    private CharacterMovement playerScript;

    protected override void Start()
    {
        base.Start();

        maxHealth = health;
        OnHealthChanged += HandlePhaseLogic;

        playerScript = UnityEngine.Object
            .FindFirstObjectByType<CharacterMovement>();
    }
    private void OnDestroy()
    {
        OnHealthChanged -= HandlePhaseLogic;
    }

    private void HandlePhaseLogic(float currentHealth)
    {
        Debug.Log($"Boss Health Changed: {currentHealth} / {maxHealth}");

        // Phase 2
        float phase2Threshold = (phaseTwoHealthPercentage / 100f) * maxHealth;
        if (!phaseTwoActivated && currentHealth <= phase2Threshold)
        {
            ActivatePhaseTwo();
        }

        // Phase 3
        float phase3Threshold = (phaseThreeHealthPercentage / 100f) * maxHealth;
        if (!phaseThreeActivated && currentHealth <= phase3Threshold)
        {
            ActivatePhaseThree();
        }
    }

    private void ActivatePhaseTwo()
    {
        phaseTwoActivated = true;
        meleeDamage = phaseTwoMeleeDamage;
        Debug.Log($"🔥 PHASE 2 ACTIVATED! Melee Damage: {meleeDamage}");
    }

    private void ActivatePhaseThree()
    {
        phaseThreeActivated = true;
        meleeDamage = phaseThreeMeleeDamage;
        Debug.Log($"💀 PHASE 3 ACTIVATED! Melee Damage: {meleeDamage}");
    }
    protected override void Die()
    {
        base.Die();

        if (playerScript != null)
        {
            playerScript.BossDied();
        }
    }
}