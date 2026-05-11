using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject cardsPanelRoot;
    private bool isOpen;

    private void Start()
    {
        SetMenu(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isOpen)
            {
                SetMenu(true);
                return;
            }
            SetMenu(false);
        }
    }

    public void Resume()
    {
        SetMenu(false);
    }

    public void OpenCardsPanel()
    {
        if (!isOpen)
        {
            SetMenu(true);
        }
        SetCardsPanelVisible(true);
    }

    public void CloseCardsPanel()
    {
        SetCardsPanelVisible(false);
    }

    private void SetMenu(bool open)
    {
        isOpen = open;
        if (menuRoot != null)
        {
            menuRoot.SetActive(open);
        }
        if (!open)
        {
            SetCardsPanelVisible(false);
        }
        Time.timeScale = open ? 0f : 1f; // pause
    }

    private void SetCardsPanelVisible(bool visible)
    {
        if (cardsPanelRoot == null)
        {
            return;
        }

        cardsPanelRoot.SetActive(visible);

        CanvasGroup canvasGroup = cardsPanelRoot.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
