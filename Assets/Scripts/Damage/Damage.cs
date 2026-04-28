using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] float damageAmount = 10f;

    public float DamageAmount => damageAmount;

    private void OnEnable()
    {
        Debug.Log($"{name} damage object enabled. Damage amount: {damageAmount}", this);
    }
}
