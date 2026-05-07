using UnityEngine;
using UnityEngine.EventSystems;

// Attach to the UI Button GameObject. Assign the Player GameObject that has PlayerMelleDefense.
public class DefendBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerMelleDefense defense;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (defense != null) defense.OnDefendButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (defense != null) defense.OnDefendButtonUp();
    }
}
