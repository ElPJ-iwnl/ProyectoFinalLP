using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string gameSceneName = "Scenes/SampleScene";

    public void PlayEasy()
    {
        if (GameSettings.I == null) return;
        GameSettings.I.ApplyEasy();
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayMedium()
    {
        if (GameSettings.I == null) return;
        GameSettings.I.ApplyMedium();
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayHard()
    {
        if (GameSettings.I == null) return;
        GameSettings.I.ApplyHard();
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
