using UnityEngine;

public class CardDropSpawner
{
    private float cardDropWorldY;

    public CardDropSpawner(float cardDropWorldY)
    {
        this.cardDropWorldY = cardDropWorldY;
    }

    public void Spawn(GameObject prefab, Vector3 position)
    {
        position.y = cardDropWorldY;
        Object.Instantiate(prefab, position, Quaternion.identity);
    }
}
