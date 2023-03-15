using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour {

    public Transform mario;
    private float leftLimit = -4.3f;
    private float rightLimit = 4.3f;
    
    // Start is called before the first frame update
    void Start() {
        if(mario == null) {
            Debug.Log("Camara. La variable mario no está correctamente inicializada");
        }        
    }
        
    void Update() {
        if(mario != null) {
            Vector3 cameraPosition = transform.position;
            cameraPosition.x = Mathf.Clamp(mario.position.x, leftLimit, rightLimit);
            transform.position = cameraPosition;
        }
    }
}
