using System;
using Characters.Health;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Characters.Team.TeamId team = Characters.Team.TeamId.Player;
    [SerializeField] private CharacterDefense defense;

    private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public Characters.Team.TeamId Team => team;
    public bool IsDefending => defense != null && defense.IsDefending;

    public event Action<float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;

        defense ??= GetComponent<CharacterDefense>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }
    private bool IsDead => currentHealth <= 0f;
    public void TakeDamage(float amount)
    {
        if (amount <= 0f ||  IsDefending)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        OnDamaged?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (IsDead)   
        {
            OnDeath?.Invoke();
            return;
        }
    }
}