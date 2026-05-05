using UnityEngine;

public partial class CardDrop
{
    const float CardDropWorldY = 1.5f;

    void EvaluateDrop(float currentHealth)
    {
        if (hasDropped || currentHealth > 0f)
        {
            return;
        }

        if (!TryResolveRandomCardPrefab(out GameObject selectedCardPrefab))
        {
            return;
        }

        hasDropped = true;
        Vector3 spawnPosition = ResolveDropPosition();
        Instantiate(selectedCardPrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 ResolveDropPosition()
    {
        Vector3 position = transform.position;
        position.y = CardDropWorldY;
        return position;
    }
}
