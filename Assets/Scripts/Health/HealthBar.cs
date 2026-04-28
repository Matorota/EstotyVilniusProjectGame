using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;
    [SerializeField] float triggerDamage = 10f;

    [Header("UI")]
    [SerializeField] Image healthFillImage;

    [Header("Trigger Damage")]
    [SerializeField] string damagingTag = "Enemy";
    
    [SerializeField] bool destroyWhenDead = false;

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        triggerDamage = Mathf.Max(0f, triggerDamage);
        UpdateHealthBar();
    }

    void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
        Debug.Log($"{name} health started: {currentHealth}/{maxHealth}", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name} trigger entered by {other.name}. Tag: {other.tag}", this);

        if (!other.CompareTag(damagingTag))
        {
            Debug.Log($"{other.name} did not damage {name}. Expected tag: {damagingTag}, actual tag: {other.tag}", this);
            return;
        }

        Damage damage = other.GetComponent<Damage>();
        float damageAmount = damage != null ? damage.DamageAmount : triggerDamage;

        Debug.Log($"{name} taking {damageAmount} damage from {other.name}", this);
        TakeDamage(damageAmount);
    }

    public void TakeDamage(float damage)
    {
        float previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        UpdateHealthBar();
        Debug.Log($"{name} health changed: {previousHealth} -> {currentHealth}", this);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        float previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0f, maxHealth);
        UpdateHealthBar();
        Debug.Log($"{name} healed: {previousHealth} -> {currentHealth}", this);
    }

    private void UpdateHealthBar()
    {
        if (healthFillImage == null)
        {
            Debug.LogWarning($"{name} has no Health Fill Image assigned.", this);
            return;
        }

        healthFillImage.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log($"{name} died.", this);

        if (destroyWhenDead)
        {
            Destroy(gameObject);
        }
    }
}
