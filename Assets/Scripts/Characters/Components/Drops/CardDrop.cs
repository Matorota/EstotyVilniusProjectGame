using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public class CardDrop : MonoBehaviour
{
    [Header("Card")]
    [SerializeField] GameObject cardObject;
    [SerializeField] bool hideCardUntilDeath = true;
    [SerializeField] bool detachCardOnReveal = true;

    Health health;
    bool hasRevealed;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (hideCardUntilDeath && cardObject != null && cardObject.scene.IsValid())
        {
            cardObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
        TryRevealCard(health.CurrentHealth);
    }

    private void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth, float _)
    {
        TryRevealCard(currentHealth);
    }

    private void TryRevealCard(float currentHealth)
    {
        if (hasRevealed || currentHealth > 0f || cardObject == null)
        {
            return;
        }

        hasRevealed = true;

        if (detachCardOnReveal && cardObject.scene.IsValid())
        {
            cardObject.transform.SetParent(null, true);
        }

        cardObject.SetActive(true);
    }
}
