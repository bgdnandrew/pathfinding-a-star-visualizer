using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour //clasa unity pt lifecycling
{
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
