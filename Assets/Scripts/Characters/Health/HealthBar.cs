using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private Characters.Health.IDamageable health;

    private void Awake()
    {
        health = GetComponent<Characters.Health.IDamageable>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += OnHealthChanged;
        Refresh(health.CurrentHealth);
    }

    private void OnDisable()
    { 
        health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
    {
        Refresh(currentHealth);
    }

    private void Refresh(float currentHealth)
    {
        float max = Mathf.Max(0.00001f, health.MaxHealth);
        healthFillImage.fillAmount = Mathf.Clamp01(currentHealth / max);
    }
}
