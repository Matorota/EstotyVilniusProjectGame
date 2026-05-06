using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDeathScreenButtonOptions : MonoBehaviour
{
    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
