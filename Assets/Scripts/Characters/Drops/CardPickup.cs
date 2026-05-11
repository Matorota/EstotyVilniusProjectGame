using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    [SerializeField] private string cardId;
    [SerializeField] private bool destroyIfAlreadyOwned;
    public string CardId => cardId;
    private Texture cardTexture;

    private void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void Awake()
    {
        RawImage rawImage = GetComponentInChildren<RawImage>(true);
        if (rawImage != null)
        {
            cardTexture = rawImage.texture;
        }
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

        if (inventory.AddCard(cardId, cardTexture))
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
