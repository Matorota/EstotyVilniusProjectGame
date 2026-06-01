using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using UnityEngine;

namespace Characters.Player.Inventory
{
    public class SelectedCardsManager : MonoBehaviour
    {
        [SerializeField] private CardInventory inventory;
        [SerializeField] private PlayerStats stats;

        public struct SelectedCardInfo
        {
            public CardModel Model;
            public CardType Type;
            public CardConfig Config;
            public bool IsActive;
        }

        public event Action OnSelectedChanged;
        [SerializeField] private int maxSelected = 3;

        private void OnEnable()
        {
            ResolveInventory();
            if (inventory != null)
                inventory.OnInventoryChanged += HandleInventoryChanged;
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void ResolveInventory()
        {
            if (inventory != null)
                return;

            inventory = CardInventory.Instance;
        }

        private void ResolvePlayerStats()
        {
            if (stats != null && stats.gameObject.activeInHierarchy)
                return;

            stats = PlayerLifecycle.Instance != null
                ? PlayerLifecycle.Instance.GetComponent<PlayerStats>()
                : null;
        }

        public bool TryEquip(CardModel model)
        {
            if (model.isEquipped)
            {
                return false;
            }

            if (inventory.GetEquippedCards().Count >= maxSelected)
            {
                return false;
            }

            if (!inventory.Equip(model))
            {
                return false;
            }

            OnSelectedChanged?.Invoke();
            return true;
        }

        public bool TryUnequip(CardModel model)
        {
            if (!inventory.Unequip(model))
            {
                return false;
            }

            model.MarkInactive();
            OnSelectedChanged?.Invoke();
            return true;
        }

        public void ClearSelected()
        {
            foreach (CardModel model in inventory.GetEquippedCards().ToList())
            {
                TryUnequip(model);
            }
        }

        public bool TryUseByIndex(int index)
        {
            ResolveInventory();
            List<CardModel> equippedCards = inventory.GetEquippedCards();
            if (index < 0 || index >= equippedCards.Count) return false;
            CardModel model = equippedCards[index];
            if (model.isActive) return false;

            ResolvePlayerStats();
            if (stats == null)
                return false;

            stats.ApplyCardEffect(model.config);
            if (isActiveAndEnabled)
            {
                StartCoroutine(ActivateForDuration(model));
            }
            else if (stats.isActiveAndEnabled)
            {
                stats.StartCoroutine(ActivateForDuration(model));
            }
            else
            {
                return false;
            }

            OnSelectedChanged?.Invoke();
            return true;
        }

        private System.Collections.IEnumerator ActivateForDuration(CardModel model)
        {
            model.MarkActive();
            OnSelectedChanged?.Invoke();
            
            yield return new WaitForSeconds(model.config.Duration);
            
            model.MarkInactive();
            OnSelectedChanged?.Invoke();
        }

        public List<SelectedCardInfo> GetSelectedCards()
        {
            ResolveInventory();
            if (inventory == null)
                return new List<SelectedCardInfo>();

            return inventory
                .GetEquippedCards()
                .Select(model => new SelectedCardInfo
                {
                    Model = model,
                    Type = model.config.Type,
                    Config = model.config,
                    IsActive = model.isActive
                })
                .ToList();
        }

        private void HandleInventoryChanged()
        {
            ResolveInventory();
            if (inventory == null)
                return;

            foreach (CardModel model in inventory.GetCollectedCards())
            {
                if (model != null && !model.isEquipped && model.isActive)
                {
                    model.MarkInactive();
                }
            }

            OnSelectedChanged?.Invoke();
        }
    }
}
