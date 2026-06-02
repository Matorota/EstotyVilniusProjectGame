using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets
{
    public class AbilityDescriptionWidget : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        public void Bind(Sprite icon, string name, string description)
        {
            if (iconImage != null)
                iconImage.sprite = icon;

            if (nameText != null)
                nameText.text = name;

            if (descriptionText != null)
                descriptionText.text = description;
        }
    }
}
