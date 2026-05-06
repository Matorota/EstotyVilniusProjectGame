using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public partial class CardDrop : MonoBehaviour
{
    [Header("Loot Drop")]
    [SerializeField] GameObject[] cardDropPrefabs;

    Health health;
    bool hasDropped;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
        EvaluateDrop(health.CurrentHealth);
    }

    private void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth, float _)
    {
        EvaluateDrop(currentHealth);
    }
}
