using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    [Header("UI de derrota")]
    [SerializeField] GameObject gameOverUI;       
    [SerializeField] float delayBeforeMenu = 2f;  
    [SerializeField] string menuSceneName = "MenuPrincipal";

    bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (!triggered && other.CompareTag("Zombie"))
        {
            triggered = true;
            StartCoroutine(GameOverSequence());
        }
    }

    System.Collections.IEnumerator GameOverSequence()
    {
        if (gameOverUI) gameOverUI.SetActive(true);


        Time.timeScale = 0f;


        yield return new WaitForSecondsRealtime(delayBeforeMenu);

        Time.timeScale = 1f;

        SceneManager.LoadScene(menuSceneName);
    }
}
