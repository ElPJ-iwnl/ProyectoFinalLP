using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = (Time.timeScale == 0f) ? 1f : 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Asegura reanudar tiempo por si estaba en pausa
            Time.timeScale = 1f;
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}
