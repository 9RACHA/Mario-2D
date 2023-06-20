using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour {
    
    private float speed = 1.8f;
    private int movementDirection = 1;
    private Vector3 velocity;
    private Animator animator;

    // Propiedad que indica si la tortuga está en fase activa o no
    private bool activa;
    public bool Activa { get { return activa; } }

    void Start() {
        if (transform.position.x > 0) {
            movementDirection = -1;
        }
        animator = GetComponent<Animator>();
        velocity = new Vector3(speed, 0, 0) * movementDirection;
        activa = true;
    }

    void Update() {
        if (transform.position.y < -10) {
            Destroy(gameObject);
        }

        if (!activa) {
            return;
        }
        Vector3 position = transform.position;
        position += velocity * Time.deltaTime;
        transform.position = position;

        Vector3 localScale = transform.localScale;
        localScale.x = -movementDirection;
        transform.localScale = localScale;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!activa) {
            return;
        }
        if (other.gameObject.CompareTag("Player") && IsGrounded()) {
            // Mario le sacudió a la tortuga desde abajo.
            // Luego la tortuga se para y se da la vuelta

            // "Desactivamos" la tortuga
            activa = false;
           
            // y la ponemos del revés
            animator.SetBool("fliping", true);
            // Por si se vuelve a poner del derecho, ponemos turning a false
            animator.SetBool("turning", false);
            // Le damos un pequeño impulso hacia arriba
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 3);

        } else if (other.gameObject.CompareTag("EnemyTeleport")) {
            Vector3 position = transform.position;
            position.x = -position.x;
            transform.position = position;
        } else if (other.gameObject.CompareTag("EnemyLift")) {     
            // Elevamos la tortuga hasta el spawnPoint del lado del que está
            // Obtenemos este punto de aparición desde el GameManager
            if (transform.position.x < 0) {
                transform.position = GameManager.instance.leftSpawnPoint;
            } else {
                transform.position = GameManager.instance.rightSpawnPoint;
            }
            ReverseMovement();
        } else if (other.gameObject.CompareTag("EnemyPipe")) {
            // Hemos detectado la entrada en uno de los triggers que hay a la
            // entrada y salida de las tuberías
            // Cambiamos la tortuga entre las capas EnemiesPipe y Enemies
            // Si está en una de ellas, la movemos a la otra
            int layerEnemiesPipe = LayerMask.NameToLayer("EnemiesPipe");
            int layerEnemies = LayerMask.NameToLayer("Enemies");
            if (gameObject.layer == layerEnemies) {
                gameObject.layer = layerEnemiesPipe;
            } else {
                gameObject.layer = layerEnemies;
            }

        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!activa) {
            if (other.gameObject.CompareTag("Player")) {
                Die(other.gameObject.GetComponent<Mario>().Velocity);
            }
            return;
        }
        
        if (other.gameObject.CompareTag("Enemy")) {
            // Paramos a la tortuga e iniciamos la animación de giro
            velocity = Vector3.zero;
            animator.SetBool("turning", true);
        }
    }

    void OnTurningCompleted() {
        animator.SetBool("turning", false);
        ReverseMovement();
    }

    private void ReverseMovement() {
        movementDirection = -movementDirection;
        velocity = new Vector3(speed, 0, 0) * movementDirection;
    }

    private bool IsGrounded() {
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y -= 0.64f;
        LayerMask mask = LayerMask.GetMask("Platform");
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, 0.05f, mask);
        if (hit.collider != null) {
            return true;
        }
        hit = Physics2D.Raycast(raycastOrigin + Vector3.right * 0.22f, Vector2.down, 0.05f, mask);
        if (hit.collider != null) {
            return true;
        }
        hit = Physics2D.Raycast(raycastOrigin + Vector3.left * 0.22f, Vector2.down, 0.05f, mask);
        if (hit.collider != null) {
            return true;
        }

        return false;
    }

    private void Die(Vector3 startVelocity) {
        animator.SetBool("fallingAway", true);
        gameObject.layer = LayerMask.NameToLayer("NoCollisions");
        GetComponent<Rigidbody2D>().velocity = new Vector2(startVelocity.x, 0);
    }
}

