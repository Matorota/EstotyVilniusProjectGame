using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public class CardDrop : MonoBehaviour
{
    private const float CardDropWorldY = 1.5f;

    [SerializeField] private GameObject[] cardDropPrefabs;

    private Health health;
    private bool hasDropped;
    private CardDropSelector selector;
    private CardDropSpawner spawner;

    private void Awake()
    {
        health = GetComponent<Health>();
        selector = new CardDropSelector();
        spawner = new CardDropSpawner(CardDropWorldY);
    }

    private void OnEnable()
    {
        health.OnHealthChanged += OnHealthChanged;
        OnHealthChanged(health.CurrentHealth);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
    {
        if (hasDropped || currentHealth > 0f)
        {
            return;
        }

        if (!selector.TrySelectPrefab(cardDropPrefabs, out GameObject selectedCardPrefab))
        {
            return;
        }

        hasDropped = true;
        spawner.Spawn(selectedCardPrefab, transform.position);
    }
}
