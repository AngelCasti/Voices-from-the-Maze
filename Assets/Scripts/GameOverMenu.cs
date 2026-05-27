using UnityEngine;
using UnityEngine.SceneManagement; // Esencial para gestionar escenas

public class GameOverMenu : MonoBehaviour
{
    // Nombre de la escena que quieres volver a cargar. Asegúrate de que coincida EXACTAMENTE.
    public string gameSceneName;

    // Nombre de la escena del menú principal (opcional, si quieres un botón para volver al menú).
    // public string mainMenuSceneName; 

    // --- FUNCIÓN PARA EL BOTÓN "RETRY" ---
    public void RetryGame()
    {
        // Si pausaste el tiempo al morir (Time.timeScale = 0), asegúrate de restablecerlo.
        Time.timeScale = 1f;

        // Carga la escena del juego nuevamente.
        // También puedes usar: SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(gameSceneName);
    }

    // --- FUNCIÓN PARA EL BOTÓN "EXIT" ---
    public void ExitGame()
    {
        // Esta función solo funciona cuando el juego está compilado y ejecutándose (standalone).
        // No cerrará el editor de Unity.
        //Application.Quit();

        // Si quieres que el botón Exit te lleve a un menú principal, usa esto en su lugar:
        SceneManager.LoadScene("Interfaz");
    }
}
