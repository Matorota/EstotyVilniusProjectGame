using UnityEngine;
using Configs;

public class PlayerStats : MonoBehaviour
{
    private float damageBonus = 0f;
    private float speedMultiplier = 1f;
    private Health health;

    public float GetDamageBonus() => damageBonus;
    public float GetSpeedMultiplier() => speedMultiplier;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void ApplyCardBuff(CardConfig cfg)
    {
        if (cfg == null)
            return;

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
    }

    public void RemoveCardBuff(CardConfig cfg)
    {
        if (cfg == null)
            return;

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