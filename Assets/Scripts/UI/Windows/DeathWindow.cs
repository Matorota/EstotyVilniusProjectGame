using UnityEngine;
using UnityEngine.UI;

public class DeathWindow : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private GameObject deathScreenGameObject;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button guildButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject guildWindowGameObject;

    private CanvasGroup canvasGroup;
    private Health playerHealth;
    private bool isVisible;
    private bool isDismissed;
    private bool isInitialized;

    private void Awake()
    {
        EnsureInitialized();
        HideWindow();
    }

    private void Update()
    {
        EnsureInitialized();

        if (playerHealth == null)
            return;

        if (isVisible && gameUIController != null && gameUIController.IsOpen)
            gameUIController.CloseMenu();

        bool isDead = playerHealth.CurrentHealth <= 0f;
        if (!isDead)
        {
            isDismissed = false;
            if (isVisible)
                HideWindow();
            return;
        }

        if (!isDismissed && !isVisible)
            ShowWindow();
    }

    private void OnDestroy()
    {
        if (!isInitialized)
            return;

        if (restartButton != null)
            restartButton.onClick.RemoveListener(HandleRestartButtonClick);

        if (guildButton != null)
            guildButton.onClick.RemoveListener(HandleGuildButtonClick);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(HandleQuitButtonClick);
    }

    public void ShowWindow()
    {
        EnsureInitialized();

        if (gameUIController != null && gameUIController.IsOpen)
            gameUIController.CloseMenu();

        GameObject screenRoot = GetScreenRoot();
        if (!screenRoot.activeSelf)
            screenRoot.SetActive(true);

        screenRoot.transform.SetAsLastSibling();
        isVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    public void HideWindow()
    {
        EnsureInitialized();
        GameObject screenRoot = GetScreenRoot();
        isVisible = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (screenRoot != gameObject)
            screenRoot.SetActive(false);
    }

    public bool IsVisible => isVisible;

    private void HandleGuildButtonClick()
    {
        isDismissed = true;

        if (guildWindowGameObject != null)
            guildWindowGameObject.SetActive(true);

        HideWindow();
    }

    private void HandleRestartButtonClick()
    {
        isDismissed = true;

        if (gameUIController != null)
            gameUIController.RestartCurrentLevel();

        HideWindow();
    }

    private void HandleQuitButtonClick()
    {
        isDismissed = true;

        if (gameUIController != null)
            gameUIController.ContinueAndOpenQuitPopup();

        HideWindow();
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
            return;

        GameObject screenRoot = GetScreenRoot();

        canvasGroup = screenRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = screenRoot.AddComponent<CanvasGroup>();

        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<Health>() : null;
        if (playerHealth == null && gameUIController != null)
            playerHealth = gameUIController.GetPlayerHealth();

        if (restartButton != null)
            restartButton.onClick.AddListener(HandleRestartButtonClick);

        if (guildButton != null)
            guildButton.onClick.AddListener(HandleGuildButtonClick);

        if (quitButton != null)
            quitButton.onClick.AddListener(HandleQuitButtonClick);

        isInitialized = true;
    }

    private GameObject GetScreenRoot()
    {
        return deathScreenGameObject != null ? deathScreenGameObject : gameObject;
    }
}
