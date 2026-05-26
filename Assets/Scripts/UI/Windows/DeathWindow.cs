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

        if (isVisible && gameUIController.IsOpen)
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

        restartButton.onClick.RemoveListener(HandleRestartButtonClick);
        guildButton.onClick.RemoveListener(HandleGuildButtonClick);
        quitButton.onClick.RemoveListener(HandleQuitButtonClick);
    }

    public void ShowWindow()
    {
        EnsureInitialized();

        if (gameUIController.IsOpen)
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

        guildWindowGameObject.SetActive(true);

        HideWindow();
    }

    private void HandleRestartButtonClick()
    {
        isDismissed = true;

        gameUIController.RestartCurrentLevel();

        HideWindow();
    }

    private void HandleQuitButtonClick()
    {
        isDismissed = true;

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

        playerHealth = gameUIController.GetPlayerHealth();

        restartButton.onClick.AddListener(HandleRestartButtonClick);
        guildButton.onClick.AddListener(HandleGuildButtonClick);
        quitButton.onClick.AddListener(HandleQuitButtonClick);

        isInitialized = true;
    }

    private GameObject GetScreenRoot()
    {
        return deathScreenGameObject != null ? deathScreenGameObject : gameObject;
    }
}
