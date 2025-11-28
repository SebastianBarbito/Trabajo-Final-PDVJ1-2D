using UnityEngine;

public class Gorgonas_Movement : MonoBehaviour
{
    public Transform player;
    public float detectionRadious = 0.5f;
    public float ataqueRadios = 2f;

    public float speed = 2;
    private float originalSpeed;
    public float fuerzaRebote = 5f;
    public int vida = 4;
    public int danio;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool ataque1;
    private bool ataque2;
    private bool enMovimiento;
    private bool corriendo;

    public float patrolTime = 4f;
    private float patrolTimer;

    private bool muerto;
    private bool recibeDanio;
    private bool playerVivo;
    private Animator animator;

    void Start()
    {
        originalSpeed = speed;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        patrolTimer = patrolTime;
    }

    void Update()
    {
        if (player != null)
        {
            Player_Movement pm = player.GetComponent<Player_Movement>();
            playerVivo = !pm.muerto;
        }

        if (playerVivo && !muerto)
        {
            if (ataque1 || ataque2 || recibeDanio)
            {
                movement = Vector2.zero;
                return;
            }

            float distance = Vector2.Distance(transform.position, player.position);

           
            if (distance <= ataqueRadios)
            {
                EjecutarAtaqueAleatorio();
            }
            else
            {
                Movimiento();
            }

        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("corriendo", corriendo);
        animator.SetBool("recibeDanio", recibeDanio);
        animator.SetBool("ataque1", ataque1);
        animator.SetBool("ataque2", ataque2);
        animator.SetBool("muerto", muerto);

    }
   


    private void Movimiento()
    {
        if (recibeDanio || ataque1 || ataque2) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // --- SEGUIR AL PLAYER SI ESTÁ CERCA ---
        if (dist < detectionRadious)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (Mathf.Abs(direction.x) < 0.3f)
            {
                direction.x = Mathf.Sign(player.position.x - transform.position.x);
            }

            movement = new Vector2(direction.x, 0);

            transform.localScale = new Vector3(direction.x < 0 ? -1 : 1, 1, 1);

            corriendo = true;
            enMovimiento = false;

            speed = originalSpeed + 3;
        }
        else
        {
            // --- PATRULLA ---
            corriendo = false;
            enMovimiento = true;

            patrolTimer -= Time.deltaTime;

            if (patrolTimer <= 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
                patrolTimer = patrolTime;
            }

            movement = new Vector2(transform.localScale.x, 0);

            speed = originalSpeed;
        }

        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }



    public void Ataque1()
    {
        if (ataque1 || recibeDanio) return;

        ataque1 = true;
        enMovimiento = false;
        corriendo = false;
        movement = Vector2.zero;

        rb.linearVelocity = Vector2.zero;
    }

    public void finAtaque1()
    {
        ataque1 = false;
    }


    public void Ataque2()
    {
        if (ataque2 || recibeDanio) return;

        ataque2 = true;
        enMovimiento = false;
        corriendo = false;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    public void finAtaque2()
    {
        ataque2 = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada_01"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direccionDanio, 2);
        }
        if (collision.CompareTag("Espada_02"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x + 1, 0);

            RecibeDanio(direccionDanio, 3);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            Player_Movement playerScript = collision.gameObject.GetComponent<Player_Movement>();

            playerScript.RecibeDanio(direccionDanio, 1);
            playerVivo = !playerScript.muerto;
            if (!playerVivo)
            {
                enMovimiento = false;
                corriendo = false;
                finAtaque1();
                finAtaque2();
            }
        }
    }


    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibeDanio) return;

        vida -= cantDanio;
        recibeDanio = true;

        // Apagamos cualquier animación activa
        ataque1 = false;
        ataque2 = false;
        enMovimiento = false;
        corriendo = false;
        movement = Vector2.zero;

        animator.SetBool("enMovimiento", false);
        animator.SetBool("corriendo", false);
        animator.SetBool("ataque1", false);
        animator.SetBool("ataque2", false);
        animator.SetBool("recibeDanio", true);

        if (vida <= 0)
        {
            muerto = true;
            Invoke("EmilinarCuerpo", 4f);
        }
        else
        {
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
        }
    }


    public void DesactivaDanio()
    {
        recibeDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void EjecutarAtaqueAleatorio()
    {
        if (recibeDanio || ataque1 || ataque2)
            return;

        // Se queda quieto durante el ataque
        enMovimiento = false;
        corriendo = false;
        movement = Vector2.zero;

        int ataqueRandom = Random.Range(0, 2); // 0 o 1

        if (ataqueRandom == 0)
            Ataque1();
        else
            Ataque2();
    }


    public void EmilinarCuerpo()
    {
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ataqueRadios);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadious);
    }
}
