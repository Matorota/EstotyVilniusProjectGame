using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterDefense))]
public class CharacterDefenseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CharacterDefense defense;

    private void Awake()
    {
        defense ??= GetComponent<CharacterDefense>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        defense?.SetUiDefense(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        defense?.SetUiDefense(false);
    }
}
