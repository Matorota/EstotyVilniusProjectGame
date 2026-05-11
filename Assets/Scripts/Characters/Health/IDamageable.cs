using System;

namespace Characters.Health
{
    public interface IDamageable
    {
        event Action<float> OnHealthChanged;
        event Action OnDeath;

        float MaxHealth { get; }
        float CurrentHealth { get; }
        Team Team { get; }
        // bool IsDefending { get; }

        void TakeDamage(float amount);
    }
}