using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeDirector : MonoBehaviour
{
    public MapDigit[] digitObjects; 
    public CodeInputPanel panel; // Referencia a tu panel

    private void Start() 
    {
        for (int i = 0; i < digitObjects.Length; i++)
        {
            if (digitObjects[i] != null)
            {
                // 1. Generamos el número aleatorio
                int val = Random.Range(0, 10);
                
                // 2. Se lo enviamos al texto en la pared
                digitObjects[i].SetDigit(val);
                
                // 3. Se lo guardamos al panel en la posición correcta (0=Rojo, 1=Verde, 2=Azul)
                if (panel != null)
                {
                    panel.correctCode[i] = val;
                }
            }
        }
    }
}
