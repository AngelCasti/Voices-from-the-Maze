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
            // Aquí puedes activar una animación o cambiar el color de los textos a verde fijo
        }
        else
        {
            Debug.Log("Código incorrecto, reiniciando...");
            // Opcional: Cambia temporalmente el color de los slots a rojo antes de resetear
            foreach(var slot in screenSlots) slot.color = Color.red; 
            Invoke("ResetPanel", 1f); 
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