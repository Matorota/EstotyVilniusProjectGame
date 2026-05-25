using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    private static  HashSet<CardPickup> ActivePickups = new(); // I need to do static without it i need to use FindObjectsOfType so one or the other i do not know witch is the better option

     private CardConfig cardConfig;
     private CardWidget cardWidget;
     private RawImage worldIconRawImage;

    private CardWidget[] widgets;

    public static List<CardPickup> GetActivePickups()
    {
        return new List<CardPickup>(ActivePickups);
    }

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

    private void OnEnable()
    {
        ActivePickups.Add(this);
    }

    private void OnDisable()
    {
        ActivePickups.Remove(this);
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
