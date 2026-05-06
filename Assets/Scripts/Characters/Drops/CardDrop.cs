using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public partial class CardDrop : MonoBehaviour
{
    [SerializeField] private GameObject[] cardDropPrefabs;

    private Health health;
    private bool hasDropped;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += OnHealthChanged;
        EvaluateDrop(health.CurrentHealth);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
    {
        EvaluateDrop(currentHealth);
    }
}
