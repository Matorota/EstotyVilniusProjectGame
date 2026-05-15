using UnityEngine;
using UnityEngine.UI;
using Configs;
using Characters.Player.Inventory;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    [SerializeField] private bool destroyIfAlreadyOwned;
    [SerializeField] private CardWidget cardWidget;
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

        CardConfig prefabConfig = cardWidget != null ? cardWidget.Config : null;
        if (prefabConfig == null)
        {
            return;
        }

        if (inventory.AddCard(prefabConfig, cardTexture))
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
