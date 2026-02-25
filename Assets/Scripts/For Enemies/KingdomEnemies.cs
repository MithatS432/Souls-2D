using UnityEngine;

public class KingdomEnemies : MonoBehaviour, IDamageable
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
