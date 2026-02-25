using UnityEngine;

public class ForestEnemies : MonoBehaviour, IDamageable
{
    public float health;
    void Start()
    {

    }

    void Update()
    {

    }
    public void GetDamage(float damage)
    {
        health -= damage;
    }
}
