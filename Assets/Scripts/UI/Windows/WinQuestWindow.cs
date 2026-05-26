using UnityEngine;
using UnityEngine.UI;

public class WinQuestWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private Button buttonResume;
    [SerializeField] private GameObject objectToAppear;
    [SerializeField] private Button endQuestButton;
    [SerializeField] private GameObject guildWindowGameObject;



    private void OnEnable()
    {

        buttonResume.onClick.AddListener(HandleResumeButtonClick);
        endQuestButton.onClick.AddListener(HandleEndQuestButtonClick);
    }

    private void OnDisable()
    {
            buttonResume.onClick.RemoveListener(HandleResumeButtonClick);
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
        gameUIController.OnEndQuestButtonPressed();
        
        if (guildWindowGameObject != null)
            guildWindowGameObject.SetActive(true);
        
        gameObject.SetActive(false);
    }
}