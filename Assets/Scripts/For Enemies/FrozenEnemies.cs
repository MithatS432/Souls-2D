using UnityEngine;

public class FrozenEnemies : MonoBehaviour, IDamageable
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
