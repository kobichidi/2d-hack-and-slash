using UnityEngine;

public class Enemy : Character, IDamageable
{
    public virtual void ApplyDamage(float amount)
    {
        CurrentHealth-=amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

}
