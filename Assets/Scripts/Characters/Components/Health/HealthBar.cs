using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image healthFillImage;
    Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
        UpdateHealthBar();
    }

    private void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float capacity)
    {
        UpdateHealthBar(current, capacity);
    }
    
    

    private void UpdateHealthBar()
    {
        if (health == null)
        {
            return;
        }

        UpdateHealthBar(health.CurrentHealth, health.HealthCapacity);
    }

    private void UpdateHealthBar(float currentHealth, float healthCapacity)
    {
        if (healthFillImage == null)
        {
            return;
        }

        float safeHealthCapacity = Mathf.Max(1f, healthCapacity);
        healthFillImage.fillAmount = currentHealth / safeHealthCapacity;
    }
}
