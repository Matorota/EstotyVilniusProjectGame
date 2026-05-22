using UnityEngine;
using UnityEngine.UI;

public class EndQuestButtonManager : MonoBehaviour
{
    [SerializeField] private Button endQuestButton;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject hudWindowRoot;

    private void OnEnable()
    {
            endQuestButton.onClick.AddListener(HandleEndQuestButtonClick);
    }

    private void OnDisable()
    {
        if (endQuestButton != null)
        {
            endQuestButton.onClick.RemoveListener(HandleEndQuestButtonClick);
        }
    }

    private void HandleEndQuestButtonClick()
    {
            winScreenRoot.SetActive(false);
            hudWindowRoot.SetActive(true);

        if (endQuestButton != null)
        {
            endQuestButton.gameObject.SetActive(false);
        }
    }
}
