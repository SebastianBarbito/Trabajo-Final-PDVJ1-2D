using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public Transform player;
    public float detectionRadious = 0.5f;
    public float ataqueRadios = 2f;
    public float speed = 2;
    public float fuerzaRebote = 5f;
    public int vida = 4;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool ataqueM_01;
    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
            if (playerVivo && !muerto)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // --- NUEVO: si está dentro del radio de ataque, ataca ---
            if (distanceToPlayer <= ataqueRadios)
            {
                Atacando();
            }
            else
            {
                Movimiento();
            }
        }


        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("ataqueM_01", ataqueM_01);
        animator.SetBool("muerto", muerto);
    }
    private void Movimiento()
    {
        if (recibiendoDanio || ataqueM_01) return; //no se mueve si ataca o recibe daño

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadious)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }


            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        if (!recibiendoDanio)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    public void Atacando()
    {
        if (ataqueM_01 || recibiendoDanio) return; // evita repetir ataque o moverse dañado

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

            playerScript.RecibeDanio(direccionDanio, 1);
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
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direccionDanio, 1);
        }
        if (collision.CompareTag("Espada_02"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x +3, 0);

            RecibeDanio(direccionDanio, 1);
        }

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
            if(vida <=0)
            {
                muerto = true;
                enMovimiento = false;
                ataqueM_01 =false;

                Invoke("EmilinarCuerpo", 4f);
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

    public void EmilinarCuerpo()
    {
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadious);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ataqueRadios);
    }
}
