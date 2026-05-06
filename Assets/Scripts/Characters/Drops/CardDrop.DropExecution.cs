using UnityEngine;

public partial class CardDrop
{
    private const float CardDropWorldY = 1.5f;

    private void EvaluateDrop(float currentHealth)
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
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = CardDropWorldY;
        Instantiate(selectedCardPrefab, spawnPosition, Quaternion.identity);
    }
}
