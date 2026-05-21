using UnityEngine;
using UnityEngine.UI;

public class WinQuestWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private Button buttonResume;
    [SerializeField] private GameObject objectToAppear;
    [SerializeField] private Button endQuestButton;



    private void OnEnable()
    {
        if (buttonResume != null)
        {
            buttonResume.onClick.AddListener(HandleResumeButtonClick);
        }
        else
        {
            Debug.LogError($"{nameof(WinQuestWindow)}: Button Resume is not assigned.", this);
        }

        if (endQuestButton != null)
        {
            endQuestButton.onClick.AddListener(HandleEndQuestButtonClick);
        }
        else
        {
            Debug.LogError($"{nameof(WinQuestWindow)}: End quest button is not assigned.", this);
        }
    }

    private void OnDisable()
    {
        if (buttonResume != null)
            buttonResume.onClick.RemoveListener(HandleResumeButtonClick);

        if (endQuestButton != null)
            endQuestButton.onClick.RemoveListener(HandleEndQuestButtonClick);
    }

    private void HandleResumeButtonClick()
    {
        objectToAppear.SetActive(true);
        gameUIController.OnContinueButtonPressed();
            
        gameObject.SetActive(false);
    }

    private void HandleEndQuestButtonClick()
    {
        gameUIController.OpenAdditionalPanel();
        endQuestButton.gameObject.SetActive(false);
    }
}