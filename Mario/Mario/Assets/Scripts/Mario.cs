using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario : MonoBehaviour {

    private float speed = 2.8f;
    private float jumpForce = 6.5f;
    private float brakeAcceleration = 7.0f;
    private Animator animator;
    private Rigidbody2D rb;

    //Booleano para controlar la inmunidad (y el parpadeo) de Mario
    //Lo hago público para poder jugar con él en directo en el inspector
    //Lo ideal sería que fuera privado
    public bool inmune;

    //Frecuencia del parpadeo de Mario
    float blinkFrequency = 10f;

    //Límites de tiempo para Mario sin matar tortugas
    private float t1 = 20;
    private float t2 = 5; 
    private float t3 = 1;

    //Tiempo acumulado por Mario sin matar tortugas
    private float t = 0;

    //Booleano para controlar que IsGrounded devuelva false
    //en los instantes siguientes después de iniciar un salto
    private bool takingOff = false;
    private bool dead = false;

    private int movementDirection = 0;
    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity {  get { return velocity; } }

    void Start() {
        t2 = t1 + t2;
        t3 = t2 + t3; 

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(BlinkRed());
    }

    void Update() {
        t += Time.deltaTime;

        if (transform.position.y < -10) {
            Destroy(gameObject);
        } 

        if (dead) {
            return;
        }

        if (t >= t2 && t < t3 ) {
            RedFlag(true);
        } else if (t >= t3) {
            MarioDie();
        }

        if (IsGrounded()) {
            animator.SetBool("falling", false);
            if (Input.GetKey(KeyCode.RightArrow) && movementDirection != -1) {
                Walk(1);
            } else if (Input.GetKey(KeyCode.LeftArrow) && movementDirection != 1) {
                Walk(-1);
            } else {
                if (Mathf.Abs(velocity.x) > 0.05f) {
                    velocity -= movementDirection * Vector3.right * brakeAcceleration * Time.deltaTime;
                    Debug.Log(velocity.x);                
                } else {
                    movementDirection = 0;
                    velocity = Vector3.zero;
                    animator.SetBool("stopping", false);
                }
                if (animator.GetBool("walking")) {
                    animator.SetBool("stopping", true);
                }
                animator.SetBool("walking", false);                
            }

            if (Input.GetKeyDown(KeyCode.Space)) {                
                Jump();
            } else if (animator.GetBool("jumping")) {
                animator.SetBool("jumping", false);
            }

        } else {
            if ((animator.GetBool("walking") || animator.GetBool("stopping")) && !animator.GetBool("jumping") && rb.velocity.y < 0) {
                animator.SetBool("falling", true);
            }
        }
        
        Vector3 newPosition = transform.position;
        newPosition.x += velocity.x * Time.deltaTime;
        transform.position = newPosition;
    }

    private void ResetTakingOff() {
        takingOff = false;
    }

    public void StartInmunity() {
        inmune = true;
        StartCoroutine(Blink());
        // TODO: Cambiar a Mario a la capa "PlayerInmune" (cuando esta exista)
        gameObject.layer = LayerMask.NameToLayer("PlayerInmune");
        Invoke("EndInmunity", 4f);
    }

    private void EndInmunity() {
        inmune = false;
        // Devolvemos a Mario a la capa "Player"
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private IEnumerator Blink() {
        Color maskColor = Color.white;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        while (inmune) {
            maskColor.a = Mathf.PingPong(Time.time * blinkFrequency, 1);
            spriteRenderer.color = maskColor;
            yield return new WaitForSeconds(0.01f);
        }
        spriteRenderer.color = Color.white;
    }

    private IEnumerator BlinkRed() {
        Color maskColor;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        while (t < t2) {
            if (t >= t1) {
                maskColor = Color.Lerp(Color.white, Color.red, RedBlinkSignal(Time.time));
                spriteRenderer.color = maskColor;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private float RedBlinkSignal(float time) {
        return Mathf.Clamp(Mathf.PingPong(time * 4, 1) * 10 - 8.5f, 0, 1);
    }

    private void Walk(int direction) {
        movementDirection = direction;
        velocity = direction * Vector3.right * speed;
        animator.SetBool("walking", true);
        animator.SetBool("stopping", false);
        Vector3 scale = transform.localScale;
        scale.x = -direction;
        transform.localScale = scale;
    }

    private void Jump() {
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        animator.SetBool("jumping", true);
        takingOff = true;
        Invoke("ResetTakingOff", 0.1f);
    }

    private bool IsGrounded() {
        if (takingOff) {
            return false;
        }
        // Si Mario se está moviendo hacia arriba, no estamos en el suelo
        if (rb.velocity.y > 0.05f) {
            return false;
        }
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

    public void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Enemy")) {
            if (other.gameObject.GetComponent<Turtle>().Activa) {
                MarioDie();
            } else {
                t = 0;
                RedFlag(false);
            }
        }
    }

    private void MarioDie() {
        animator.SetBool("gettingShocked", true);
        dead = true;
        rb.velocity = Vector3.zero;
        rb.gravityScale = 0;
        gameObject.layer = LayerMask.NameToLayer("NoCollisions");
        // Anotamos la vida perdida en el GameManager,
        // que spawneará otro Mario si quedan vidas
        GameManager.instance.MarioDead();
    }

    private void RedFlag(bool red) {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (red) {
            spriteRenderer.color = Color.red;
        } else {
            spriteRenderer.color = Color.white;
        }
    }

    public void OnGettingSockedCompleted() {
        animator.SetBool("fallingDead", true);
        rb.gravityScale = 1;
    }
}
