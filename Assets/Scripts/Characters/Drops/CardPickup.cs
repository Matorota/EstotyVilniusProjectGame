using UnityEngine;
using UnityEngine.UI;
using Configs;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    [SerializeField] private string cardId;
    [SerializeField] private bool destroyIfAlreadyOwned;
    [SerializeField] private CardWidget cardWidget;
    public string CardId => cardId;
    private Texture cardTexture;

    private void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void Awake()
    {
        cardWidget ??= GetComponent<CardWidget>();

        RawImage rawImage = GetComponentInChildren<RawImage>(true);
        if (rawImage != null && rawImage.texture != null)
        {
            cardTexture = rawImage.texture;
            return;
        }

        Image image = GetComponentInChildren<Image>(true);
        if (image != null && image.sprite != null)
        {
            cardTexture = image.sprite.texture;
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

        string pickedCardId = ResolveCardId(cardWidget != null ? cardWidget.Config : null);
        if (inventory.AddCard(pickedCardId, cardTexture))
        {
            Destroy(gameObject);
            return;
        }

        if (destroyIfAlreadyOwned)
        {
            Destroy(gameObject);
        }
    }

    private string ResolveCardId(CardConfig pickedCardConfig)
    {
        if (!string.IsNullOrWhiteSpace(cardId))
        {
            return cardId;
        }

        if (pickedCardConfig != null)
        {
            return pickedCardConfig.Type.ToString();
        }

        return string.Empty;
    }
}
