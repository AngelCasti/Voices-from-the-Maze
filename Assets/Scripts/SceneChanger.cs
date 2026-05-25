using UnityEngine;
using UnityEngine.SceneManagement; // Librería para cambiar de escena

public class SceneChanger : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Escribe el nombre exacto de la escena a la que quieres ir")]
    public string nombreDeEscena;

    // Esta función será llamada por el botón del Canvas
    public void CambiarAEscena()
    {
        if (!string.IsNullOrEmpty(nombreDeEscena))
        {
            // Cargamos la nueva escena
            SceneManager.LoadScene(nombreDeEscena);
        }
        else
        {
            Debug.LogWarning("No has escrito ningún nombre de escena en el script.");
        }
    }
}