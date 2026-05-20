using UnityEngine;

public class ShowUiWhenAllEnemiesDead : MonoBehaviour
{

    
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private bool pauseGameOnShow = true;

    private void Awake()
    {
        enabled = false;
        Debug.Log("ShowUiWhenAllEnemiesDead: Disabled - use WinScreen.cs + EndQuestButtonManager.cs instead");
    }
}
