using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDefenseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CharacterDefense defense;

    public void OnPointerDown(PointerEventData eventData)
    {
        defense?.SetUiDefense(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        defense?.SetUiDefense(false);
    }

    private void OnDisable()
    {
        defense?.SetUiDefense(false);
    }
}