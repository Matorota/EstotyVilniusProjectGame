using System;
using Characters.Health;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Characters.Team.TeamId team = Characters.Team.TeamId.Player;

    private float currentHealth;
    private CharacterAttackAnimation attackAnimation;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public Characters.Team.TeamId Team => team;
    public bool IsDefending => GetComponent<CharacterDefense>()?.IsDefending == true || GetComponentInParent<CharacterDefense>()?.IsDefending == true;

    public event Action<float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action OnDeath;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        attackAnimation = GetComponent<CharacterAttackAnimation>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    private bool IsDead => currentHealth <= 0f;

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        if (IsDefending) return;
        if (IsDead) return;

        float updatedHealth = currentHealth - amount;
        currentHealth = updatedHealth <= 0f ? 0f : updatedHealth;
        attackAnimation?.TryPlayHit();
        OnDamaged?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (IsDead)
        {
            OnDeath?.Invoke();
            return;
        }
    }
}
