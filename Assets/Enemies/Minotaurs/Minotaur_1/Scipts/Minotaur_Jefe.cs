using UnityEngine;
using System.Collections;

public class Minotaur_Jefe : MonoBehaviour
{
    public Transform player;
    public float detectionRadious = 0.5f;      // AREA para empezar a seguir al jugador
    public float ataqueRadios = 2f;            // AREA para atacar
    public float speed = 2;
    public float fuerzaRebote = 5f;
    public int danioHacha = 2;                 // daño del hacha

    public int vida = 10;                      // vida total

    // FASES
    private bool cambioFase2;
    private bool cambioFase3;

    private Color baseColor;
    private SpriteRenderer sr;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool ataqueM_01;
    private bool enMovimiento;

    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo;
    private Animator animator;

    void Start()
    {

        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;                   // guardamos color original
    }

    void Update()
    {
        if (playerVivo && !muerto)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Solo se mueve si el jugador entra a su área
            if (distanceToPlayer <= ataqueRadios)
            {
                Atacando();
            }
            else if (distanceToPlayer <= detectionRadious)
            {
                Movimiento();
            }
            else
            {
                enMovimiento = false;
                movement = Vector2.zero;
            }
        }

        // Animaciones
        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("ataqueM_01", ataqueM_01);
        animator.SetBool("muerto", muerto);
    }

    private void Movimiento()
    {
        if (recibiendoDanio || ataqueM_01) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // mirar al jugador
        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        movement = new Vector2(direction.x, 0);
        enMovimiento = true;

        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    public void Atacando()
    {
        if (ataqueM_01 || recibiendoDanio) return;

        ataqueM_01 = true;
        enMovimiento = false;
        movement = Vector2.zero;
    }

    public void finAtaque()
    {
        ataqueM_01 = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            Player_Movement playerScript = collision.gameObject.GetComponent<Player_Movement>();

            playerScript.RecibeDanio(direccionDanio, danioHacha);
            playerVivo = !playerScript.muerto;

            if (!playerVivo)
            {
                enMovimiento = false;
                finAtaque();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada_01"))
            RecibeDanio(collision.transform.position, 1);

        if (collision.CompareTag("Espada_02"))
            RecibeDanio(collision.transform.position + new Vector3(3, 0), 1);

        if (collision.CompareTag("Player"))
        {
            Player_Movement playerScript = collision.gameObject.GetComponent<Player_Movement>();
            playerVivo = !playerScript.muerto;

            if (!playerVivo)
            {
                ataqueM_01 = false;
                enMovimiento = false;
            }
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            vida -= cantDanio;
            recibiendoDanio = true;

            RevisarFases();  //  aquí revisamos para cambiar fase

            if (vida <= 0)
            {
                muerto = true;
                enMovimiento = false;
                ataqueM_01 = false;
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    void RevisarFases()
    {
        // -------------------------
        // FASE 2 (vida menor al 66%)
        // -------------------------
        if (!cambioFase2 && vida <= (10 * 0.66f))
        {
            cambioFase2 = true;

            speed *= 0.8f;          // más lento
            danioHacha += 1;        // más daño
            vida += 3;              // pequeña curación

            // más rojo
            sr.color = new Color(1f, 0.5f, 0.5f);

            Debug.Log("FASE 2 ACTIVADA");
        }

        // -------------------------
        // FASE 3 (vida menor al 33%)
        // -------------------------
        if (!cambioFase3 && vida <= (10 * 0.33f))
        {
            cambioFase3 = true;

            speed *= 0.6f;
            danioHacha += 2;
            vida += 2;

            // rojo más intenso


            sr.color = new Color(1f, 0.2f, 0.2f);

            Debug.Log("FASE 3 ACTIVADA");
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadious);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ataqueRadios);
    }

}
