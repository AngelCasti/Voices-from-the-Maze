using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapDigit : MonoBehaviour
{
    public TMP_Text textComponent; 
    public Color digitColor; 

    // Este método lo llamará el CodeDirector
    public void SetDigit(int number)
    {
        textComponent.text = number.ToString();
        textComponent.color = digitColor;
        // Forzamos la actualización de la malla para que el color se aplique
        textComponent.ForceMeshUpdate(); 
    }
}
