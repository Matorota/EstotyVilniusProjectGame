using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += OnHealthChanged;
        UpdateHealthBar(health.CurrentHealth, health.MaxHealth);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
    {
        UpdateHealthBar(currentHealth, health.MaxHealth);
    }
    
    

    private void UpdateHealthBar(float currentHealth, float healthCapacity)
    {
        float safeHealthCapacity = Mathf.Max(1f, healthCapacity);
        healthFillImage.fillAmount = currentHealth / safeHealthCapacity;
    }
}
