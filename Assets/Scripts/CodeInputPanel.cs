using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CodeInputPanel : MonoBehaviour
{
    // Asegúrate de que no haya nada extraño aquí
    public TMP_Text[] screenSlots; 
    public int[] correctCode = new int[3]; 
    
    private int[] playerAttempt = new int[3];
    private int currentIndex = 0;

    [Header("Colores Originales")]
    public Color[] originalColors = new Color[3];

    public void AddDigit(int digit)
    {
        if (currentIndex < 3)
        {
            playerAttempt[currentIndex] = digit;
            if (screenSlots[currentIndex] != null)
            {
                screenSlots[currentIndex].text = digit.ToString();
            }
            currentIndex++;

            if (currentIndex == 3)
            {
                CheckCode();
            }
        }
    }

    private void CheckCode()
    {
        bool isCorrect = (playerAttempt[0] == correctCode[0] && 
                        playerAttempt[1] == correctCode[1] && 
                        playerAttempt[2] == correctCode[2]);

        if (isCorrect)
        {
            Debug.Log("¡Puerta abierta!");
            // Ponemos los 3 en verde al acertar
            for(int i = 0; i < screenSlots.Length; i++)
            {
                screenSlots[i].color = Color.green;
            }
        }
        else
        {
            Debug.Log("Código incorrecto, mostrando error...");
            // Ponemos todos en rojo inmediatamente
            foreach(var slot in screenSlots) slot.color = Color.red;
            
            // Esperamos 1.5 segundos y llamamos a la función que restaura los colores
            Invoke("RestoreOriginalColors", 1.5f);
        }
    }

    private void RestoreOriginalColors()
    {
        // Limpiamos el intento (asumiendo que tienes una función para resetear el input)
        ResetPanel(); 

        // Restauramos cada uno a su color original definido en el array
        for(int i = 0; i < screenSlots.Length; i++)
        {
            screenSlots[i].color = originalColors[i];
        }
    }

    private void ResetPanel()
    {
        currentIndex = 0;
        foreach (var slot in screenSlots) 
        {
            if (slot != null) slot.text = "_";
        }
    }
}