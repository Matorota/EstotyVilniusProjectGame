using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDefenseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PlayerLifecycle playerLifecycle;

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
        if (playerLifecycle == null)
            playerLifecycle = FindFirstObjectByType<PlayerLifecycle>();

        if (playerLifecycle == null)
            return null;

        return playerLifecycle.GetComponent<CharacterDefense>();
    }
}