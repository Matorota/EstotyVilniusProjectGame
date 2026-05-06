using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    [Header("Card")]
    [SerializeField] string cardId;
    [SerializeField] bool destroyIfAlreadyOwned;

    private void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TryGetInventory(other, out CardInventory inventory))
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

    bool TryGetInventory(Collider other, out CardInventory inventory)
    {
        inventory =
            other.GetComponent<CardInventory>() ??
            other.GetComponentInParent<CardInventory>() ??
            other.GetComponentInChildren<CardInventory>();

        return inventory != null;
    }
}
