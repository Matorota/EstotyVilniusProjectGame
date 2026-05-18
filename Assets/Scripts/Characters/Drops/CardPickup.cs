using UnityEngine;
using Configs;
using Characters.Player.Inventory;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    [SerializeField] private CardConfig cardConfig;
    [SerializeField] private CardWidget cardWidget;
    [SerializeField] private RawImage worldIconRawImage;

    private CardWidget[] widgets;

    private void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void Awake()
    {
        cardWidget ??= GetComponent<CardWidget>() ?? GetComponentInChildren<CardWidget>(true);
        worldIconRawImage ??= GetComponentInChildren<RawImage>(true);
        widgets = GetComponentsInChildren<CardWidget>(true);
        ApplyConfigToVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        CardInventory inventory =
            other.GetComponent<CardInventory>() ??
            other.GetComponentInParent<CardInventory>();

        if (inventory == null)
        {
            return;
        }

        CardConfig configToCollect = cardConfig ?? cardWidget?.Config;
        if (configToCollect == null)
        {
            return;
        }

        if (inventory.Collect(new CardModel { config = configToCollect }))
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(CardConfig config)
    {
        cardConfig = config;
        cardWidget ??= GetComponent<CardWidget>() ?? GetComponentInChildren<CardWidget>(true);
        worldIconRawImage ??= GetComponentInChildren<RawImage>(true);
        widgets = GetComponentsInChildren<CardWidget>(true);
        ApplyConfigToVisuals();
    }

    private void ApplyConfigToVisuals()
    {
        if (cardConfig == null)
        {
            return;
        }

        if (cardWidget != null)
        {
            cardWidget.Setup(cardConfig);
        }

        if (widgets == null)
        {
            return;
        }

        for (int i = 0; i < widgets.Length; i++)
        {
            CardWidget widget = widgets[i];
            if (widget != null)
            {
                widget.Setup(cardConfig);
            }
        }

        if (cardConfig.Image != null)
        {
            if (worldIconRawImage != null)
            {
                worldIconRawImage.texture = cardConfig.Image.texture;
            }
        }
    }
}
