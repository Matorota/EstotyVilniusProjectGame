using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private IDamageable health;

    private void Awake()
    {
        health = GetComponent<IDamageable>();
        if (health == null || healthFillImage == null)
        {
            Debug.LogWarning($"{nameof(HealthBar)} on {name} is missing references.");
            enabled = false;
            return;
        }

        if (healthFillImage != null && healthFillImage.color == Color.white) // was having issues with health bar being white
        {
            healthFillImage.color = Color.red;
        }
    }

    private void OnEnable()
    {
        if (health == null) return;
        health.OnHealthChanged += OnHealthChanged;
        Refresh(health.CurrentHealth);
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
        }
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
