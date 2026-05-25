using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSafety : MonoBehaviour
{
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Este evento se dispara siempre que el Character Controller choca con algo
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Fuerza al controlador a detenerse si detecta una colisión
        // Esto evita que el script de movimiento "empuje" al jugador a través del muro
        Rigidbody body = hit.collider.attachedRigidbody;
        
        if (body != null && !body.isKinematic)
        {
            body.velocity += controller.velocity * 0.5f;
        }
    }
}
