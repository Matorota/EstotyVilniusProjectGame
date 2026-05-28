using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDefenseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        ResolveDefense()?.SetUiDefense(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResolveDefense()?.SetUiDefense(false);
    }

    private void OnDisable()
    {
        ResolveDefense()?.SetUiDefense(false);
    }

    private CharacterDefense ResolveDefense()
    {
        if (PlayerLifecycle.Instance == null)
            return null;
        return PlayerLifecycle.Instance.GetComponent<CharacterDefense>();
    }
}