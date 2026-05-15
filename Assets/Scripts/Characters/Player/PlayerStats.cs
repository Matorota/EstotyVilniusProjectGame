using System.Collections;
using UnityEngine;
using Configs;

public class PlayerStats : MonoBehaviour
{
    private float damageBonus = 0f; // flat add to melee damage
    private float speedMultiplier = 1f;
    private Health health;

    public float GetDamageBonus() => damageBonus;
    public float GetSpeedMultiplier() => speedMultiplier;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void ApplyCardEffect(CardConfig cfg)
    {
        if (cfg == null) return;
        StartCoroutine(ApplyTemporary(cfg));
    }

    private IEnumerator ApplyTemporary(CardConfig cfg)
    {
        float duration = Mathf.Max(0f, cfg.Duration);
        switch (cfg.Type)
        {
            case Characters.Player.Inventory.CardType.Damage:
                damageBonus += cfg.Value;
                break;
            case Characters.Player.Inventory.CardType.SpeedUp:
                speedMultiplier += cfg.Value;
                break;
            case Characters.Player.Inventory.CardType.Health:
                if (health != null)
                {
                    health.AddMaxHealth(cfg.Value);
                    health.Heal(cfg.Value);
                }
                break;
        }

        yield return new WaitForSeconds(duration);

        switch (cfg.Type)
        {
            case Characters.Player.Inventory.CardType.Damage:
                damageBonus -= cfg.Value;
                break;
            case Characters.Player.Inventory.CardType.SpeedUp:
                speedMultiplier = Mathf.Max(1f, speedMultiplier - cfg.Value);
                break;
            case Characters.Player.Inventory.CardType.Health:
                if (health != null)
                {
                    health.RemoveMaxHealth(cfg.Value);
                }
                break;
        }
    }
}