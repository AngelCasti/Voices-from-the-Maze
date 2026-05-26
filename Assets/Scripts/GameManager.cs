using UnityEngine;
using UnityEngine.SceneManagement; // Por si quieres usar la función de reiniciar

public class GameManager : MonoBehaviour
{
    [Header("UI de Derrota")]
    [Tooltip("Arrastra aquí el objeto vacío Menu_Derrota que creaste")]
    public GameObject menuDerrota;

    void Start()
    {
        // Nos aseguramos de que el menú empiece desactivado al iniciar la partida
        if (menuDerrota != null)
        {
            menuDerrota.SetActive(false);
        }
    }

    // Esta es la función que llamaremos desde donde el jugador pierda
    public void Derrota()
    {
        if (menuDerrota != null)
        {
            // 1. Buscamos la cámara principal de VR (los ojos del jugador)
            Transform camaraVR = Camera.main.transform;

            if (camaraVR != null)
            {
                // 2. Calculamos una posición a 2 metros en frente de la cámara
                Vector3 posicionFrente = camaraVR.position + (camaraVR.forward * 2.0f);
                
                // Ajustamos la altura del menú para que quede a la altura de los ojos
                posicionFrente.y = camaraVR.position.y; 

                // 3. Movemos el menú a esa posición
                menuDerrota.transform.position = posicionFrente;

                // 4. Hacemos que el menú mire al jugador
                // Usamos una rotación hacia la cámara, pero la volteamos 180 grados para que no quede al revés
                menuDerrota.transform.LookAt(camaraVR);
                menuDerrota.transform.Rotate(0, 180, 0);
            }

            // 5. Finalmente, activamos el menú ya posicionado en tu cara
            menuDerrota.SetActive(true);
        }
    }

    // Función extra para el botón de Reiniciar
    public void ReiniciarNivel()
    {
        // Quita la pausa si la pusiste
        Time.timeScale = 1f; 
        
        // Vuelve a cargar la escena en la que estás actualmente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // FUNCIÓN PARA EL BOTÓN "RETRY" (Reiniciar)
    public void ReiniciarJuego()
    {
        // Si pausaste el tiempo al morir, lo restauramos a la normalidad
        Time.timeScale = 1f; 
        
        // Volvemas a cargar la escena en la que el jugador está jugando actualmente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // FUNCIÓN PARA EL BOTÓN "REGRESAR" (Menú Principal)
    public void RegresarAlMenu()
    {
        // Aseguramos que el tiempo corra normal antes de salir
        Time.timeScale = 1f; 
        
        // Cargamos tu escena principal. 
        // Reemplaza "MenuPrincipal" por el nombre exacto de tu escena del menú.
        SceneManager.LoadScene("Interfaz"); 
    }
}