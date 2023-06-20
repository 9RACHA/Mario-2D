using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour {

    public Transform mario; // Referencia al objeto del personaje "Mario" para que lo siga la camara
    private float leftLimit = -4.3f; // Límite izquierdo de la cámara
    private float rightLimit = 4.3f; // Límite derecho de la cámara

    void Start() {
        if (mario == null) {
            Debug.Log("Camara: La variable 'mario' no está correctamente inicializada");
        }
    }

    void Update() {
        if (mario != null) {
            Vector3 cameraPosition = transform.position;
            cameraPosition.x = Mathf.Clamp(mario.position.x, leftLimit, rightLimit); // Limitar la posición X de la cámara dentro de los límites izquierdo y derecho
            transform.position = cameraPosition; // Actualizar la posición de la cámara
        }
    }
}

