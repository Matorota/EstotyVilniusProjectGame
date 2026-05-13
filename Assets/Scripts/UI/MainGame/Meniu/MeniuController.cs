using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject cardsPanelRoot;
    [SerializeField] private GameObject quildPanelRoot;
    [SerializeField] private GameObject quildBacktoquildPanelRoot;
    [SerializeField] private GameObject hudWindowRoot;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject deathScreenRoot;
    private bool isOpen;

    private void Start()
    {
        SetMenu(false);
    }

    private void Update()
    {
        if (IsEndScreenActive())
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetMenu(!isOpen);
        }
    }

    public void Resume()
    {
        SetMenu(false);
    }

    public void OpenCardsPanel()
    {
        OpenPanel(cardsPanelRoot);
    }

    public void CloseCardsPanel()
    {
        SetPanelVisible(cardsPanelRoot, false);
    }

    public void OpenAdditionalPanel()
    {
        OpenPanel(quildPanelRoot);
    }

    public void CloseAdditionalPanel()
    {
        SetPanelVisible(quildPanelRoot, false);
    }

    public void OpenBacktoquildPanel()
    {
        OpenPanel(quildBacktoquildPanelRoot);
    }

    public void CloseBacktoquildPanel()
    {
        SetPanelVisible(quildBacktoquildPanelRoot, false);
    }

    private void SetMenu(bool open)
    {
        if (open && IsEndScreenActive())
        {
            return;
        }

        isOpen = open;
        if (menuRoot != null)
        {
            menuRoot.SetActive(open);
        }
        if (hudWindowRoot != null)
        {
            hudWindowRoot.SetActive(!open);
        }
        if (!open)
        {
            CloseAllPanels();
        }
        Time.timeScale = open ? 0f : 1f; // pause
    }

    private bool IsEndScreenActive()
    {
        bool isWinScreenVisible = winScreenRoot != null && winScreenRoot.activeInHierarchy;
        bool isDeathScreenVisible = deathScreenRoot != null && deathScreenRoot.activeInHierarchy;
        return isWinScreenVisible || isDeathScreenVisible;
    }

    private void OpenPanel(GameObject panelRoot)
    {
        if (!isOpen)
        {
            SetMenu(true);
        }

        CloseAllPanels();
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(deathScreenRoot, false);
        SetPanelVisible(panelRoot, true);
    }

    private void CloseAllPanels()
    {
        SetPanelVisible(cardsPanelRoot, false);
        SetPanelVisible(quildPanelRoot, false);
        SetPanelVisible(quildBacktoquildPanelRoot, false);
    }

    private void SetPanelVisible(GameObject panelRoot, bool visible)
    {
        if (panelRoot == null)
        {
            return;
        }

        panelRoot.SetActive(visible);

        CanvasGroup canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }

    private void SetActiveIfAssigned(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}
