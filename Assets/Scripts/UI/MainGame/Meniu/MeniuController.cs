using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject quitPopupRoot;
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
        SetPanelVisible(quitPopupRoot, false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            WinScreen win = null;
            if (winScreenRoot != null)
            {
                win = winScreenRoot.GetComponent<WinScreen>();
            }
            if (win == null)
            {
                win = FindObjectOfType<WinScreen>();
            }

            if (win != null && win.HasWon)
            {
                win.ShowWinScreen();
                return;
            }

            if (IsEndScreenActive())
            {
                return;
            }

            SetMenu(!isOpen);
        }
    }

    public void Resume()
    {
        isOpen = false;
        SetActiveIfAssigned(menuRoot, false);
        SetPanelVisible(quitPopupRoot, false);
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(deathScreenRoot, false);
        SetActiveIfAssigned(hudWindowRoot, true);
        CloseAllPanels();
        Time.timeScale = 1f;
    }

    public void ContinueAndOpenQuitPopup()
    {
        Resume();
    }

    public void OpenMenu()
    {
        SetMenu(true);
    }

    public void CloseMenu()
    {
        SetMenu(false);
    }

    public void ToggleMenu()
    {
        SetMenu(!isOpen);
    }

    public void OpenQuitPopup()
    {
        if (!isOpen)
        {
            SetMenu(true);
        }

        SetPanelVisible(quitPopupRoot, true);
    }

    public void CloseQuitPopup()
    {
        SetPanelVisible(quitPopupRoot, false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
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
            SetPanelVisible(quitPopupRoot, false);
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
        SetPanelVisible(quitPopupRoot, false);
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
