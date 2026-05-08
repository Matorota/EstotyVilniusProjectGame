using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public class CardDrop : MonoBehaviour
{
    private const float CardDropWorldY = 1.5f;

    [SerializeField] private GameObject[] cardDropPrefabs;

    private Characters.Health.IDamageable health;
    private bool hasDropped;
    private CardDropSelector selector;
    private CardDropSpawner spawner;

    private void Awake()
    {
        health = GetComponent<Characters.Health.IDamageable>();
        selector = new CardDropSelector();
        spawner = new CardDropSpawner(CardDropWorldY);
    }

    private void OnEnable()
    {
        health.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        if (hasDropped)
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
