using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image healthFillImage;
    Health health;

    void OnValidate()
    {
        CacheDependencies();
        UpdateHealthBar();
    }

    void Awake()
    {
        CacheDependencies();
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.HealthChanged += OnHealthChanged;
            UpdateHealthBar();
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.HealthChanged -= OnHealthChanged;
        }
    }

    void OnHealthChanged(float current, float capacity)
    {
        UpdateHealthBar(current, capacity);
    }
    
    

    void UpdateHealthBar()
    {
        if (health == null)
        {
            return;
        }

        UpdateHealthBar(health.CurrentHealth, health.HealthCapacity);
    }

    void UpdateHealthBar(float currentHealth, float healthCapacity)
    {
        if (healthFillImage == null)
        {
            return;
        }

        float safeHealthCapacity = Mathf.Max(1f, healthCapacity);
        healthFillImage.fillAmount = currentHealth / safeHealthCapacity;
    }

    void CacheDependencies()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }
}
