using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    [SerializeField] private string cardId;
    [SerializeField] private bool destroyIfAlreadyOwned;
    public string CardId => cardId;

    private void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CardInventory inventory =
            other.GetComponent<CardInventory>() ??
            other.GetComponentInParent<CardInventory>() ??
            other.GetComponentInChildren<CardInventory>();
        if (inventory == null)
        {
            return;
        }

        if (inventory.AddCard(cardId))
        {
            Destroy(gameObject);
            return;
        }

        if (destroyIfAlreadyOwned)
        {
            Destroy(gameObject);
        }
    }
}
