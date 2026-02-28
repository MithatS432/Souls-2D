using UnityEngine;

public class ForestBoss : ForestEnemies
{
    [Header("Phase Thresholds")]
    [SerializeField] private float phaseTwoHealthThreshold = 70f;
    [SerializeField] private float phaseThreeHealthThreshold = 30f;

    [Header("Phase Damage Settings")]
    [SerializeField] private float phaseTwoMeleeDamage = 30f;
    [SerializeField] private float phaseThreeMeleeDamage = 50f;

    private bool phaseTwoActivated = false;
    private bool phaseThreeActivated = false;

    protected override void Start()
    {
        base.Start();
        OnHealthChanged += HandlePhaseLogic;
    }

    private void OnDestroy()
    {
        OnHealthChanged -= HandlePhaseLogic;
    }

    private void HandlePhaseLogic(float currentHealth)
    {
        if (!phaseTwoActivated && currentHealth <= phaseTwoHealthThreshold)
        {
            ActivatePhaseTwo();
        }

        if (!phaseThreeActivated && currentHealth <= phaseThreeHealthThreshold)
        {
            ActivatePhaseThree();
        }
    }

    private void ActivatePhaseTwo()
    {
        phaseTwoActivated = true;
        meleeDamage = phaseTwoMeleeDamage;
        Debug.Log("Forest Boss → Phase 2 Activated (Damage Increased)");
    }

    private void ActivatePhaseThree()
    {
        phaseThreeActivated = true;
        meleeDamage = phaseThreeMeleeDamage;
        Debug.Log("Forest Boss → Phase 3 Activated (Damage Increased)");
    }
}