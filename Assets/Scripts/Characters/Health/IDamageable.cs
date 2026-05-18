using System;

public interface IDamageable
{
    event Action<float> OnHealthChanged;
    event Action OnDeath;

    float MaxHealth { get; }
    float CurrentHealth { get; }
    Team Team { get; }

    void TakeDamage(float amount);
}
