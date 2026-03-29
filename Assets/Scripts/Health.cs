using UnityEngine;

public class Health
{
    public int Max { get; private set; }
    public int Current { get; private set; }

    public Health(int maxHealth)
    {
        Max = maxHealth;
        Current = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (Current <= 0) return;
        
        Current -= damage;
        Current = Mathf.Max(Current, 0);
    }

    public void Heal(int healAmount)
    {
        Current += healAmount;
        Current = Mathf.Clamp(Current, 0, Max);
    }

    public void FullHeal()
    {
        Current = Max;
    }

    public bool IsDead() => Current <= 0;
}
