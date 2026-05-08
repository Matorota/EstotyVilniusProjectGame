using System;
using Characters.Team;

namespace Characters.Health
{
    public interface IDamageable
    {
        event Action<float> OnHealthChanged;
        event Action<float> OnDamaged;
        event Action OnDeath;
        float MaxHealth { get; }
        float CurrentHealth { get; }
        void TakeDamage(float amount);
        TeamId Team { get; }
        bool IsDefending { get; }
    }
}
