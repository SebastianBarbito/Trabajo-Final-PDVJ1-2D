using UnityEngine;

public class Jefe_Gorgona : MonoBehaviour
{
    [Header("FASE 2")]
    public bool enFase2 = false;
    public float multiplicadorTamano = 1.2f;
    public int bonusDanioFase2 = 2;
    public float bonusVelocidadFase2 = 1f;

    [Header("INVOCACION AL MORIR")]
    public GameObject gorgonaPrefab1;
    public GameObject gorgonaPrefab2;
    public Transform spawn1;
    public Transform spawn2;


    public Transform player;
    public float detectionRadious = 0.5f;
    public float ataqueRadios = 2f;
    public float pretrificarRadios = 3f;

    public float tiempoEntreEspecial = 4f;  // cada cuántos segundos intenta petrificar
    private float temporizadorEspecial;

    public float speed = 2;
    private float originalSpeed;
    public float fuerzaRebote = 5f;
    public int vidaMaxima;
    public int vida = 15;
    public int danio;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool ataque1;
    private bool ataque2;
    private bool ataqueEspecial;
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
        vidaMaxima = vida;
        temporizadorEspecial = tiempoEntreEspecial;

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

        if (playerVivo && !muerto)
        {
            if (ataque1 || ataque2 || recibeDanio || ataqueEspecial)
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

            // ---- ATAQUE ESPECIAL AUTOMÁTICO ----
            temporizadorEspecial -= Time.deltaTime;

            if (temporizadorEspecial <= 0 && !ataqueEspecial && !ataque1 && !ataque2 && !recibeDanio)
            {
                float dist = Vector2.Distance(transform.position, player.position);

                if (dist <= pretrificarRadios)
                {
                    AtaqueEspecial();                   // Ejecuta el ataque
                    temporizadorEspecial = tiempoEntreEspecial;  // Reinicia el tiempo
                }
                else
                {
                    // Si está lejos, espera un poco y vuelve a intentar
                    temporizadorEspecial = 1.5f;
                }
            }


        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("corriendo", corriendo);
        animator.SetBool("recibeDanio", recibeDanio);
        animator.SetBool("ataque1_JefeGor", ataque1);
        animator.SetBool("ataque2_JefeGor", ataque2);
        animator.SetBool("muerto", muerto);
        animator.SetBool("ataqueEsp_JefeGor", ataqueEspecial);
    }


    private void Movimiento()
    {
        if (recibeDanio || ataque1 || ataque2 || ataqueEspecial) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // --- SEGUIR AL PLAYER SI ESTÁ CERCA ---
        if (dist < detectionRadious)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Evita que el normalizado genere (0,0) al estar demasiado cerca
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
        animator.SetBool("ataque1_JefeGor", false);
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
        animator.SetBool("ataque2_JefeGor", false);
    }

    public void AtaqueEspecial()
    {
        if (ataqueEspecial || recibeDanio) return;
        ataqueEspecial = true;
        enMovimiento = false;
        corriendo = false;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
    public void finAtaqueEspecial()
    {
        ataqueEspecial = false;
        animator.SetBool("ataqueEsp_JefeGor", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada_01"))
        {
            Vector2 direccionDanio = (transform.position - collision.transform.position).normalized;
            RecibeDanio(direccionDanio, 2, 4f);   // Rebote débil
        }
        if (collision.CompareTag("Espada_02"))
        {
            Vector2 direccionDanio = (transform.position - collision.transform.position).normalized;
            RecibeDanio(direccionDanio, 3, 2f);   // Rebote más fuerte
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

                animator.SetBool("enMovimiento", false);
                animator.SetBool("corriendo", false);
                animator.SetBool("ataque1_JefeGor", false);
                animator.SetBool("ataque2_JefeGor", false);
                animator.SetBool("ataqueEsp_JefeGor", false);
            }
        }
    }


    public void RecibeDanio(Vector2 direccion, int cantDanio, float fuerzaRebotePersonalizada)
    {
        if (recibeDanio) return;

        if (enFase2 == true)
            cantDanio = Mathf.RoundToInt(cantDanio * 0.8f); // Recibe un poco menos

        vida -= cantDanio;
        recibeDanio = true;


        ataque1 = false;
        ataque2 = false;
        ataqueEspecial = false;
        enMovimiento = false;
        corriendo = false;
        movement = Vector2.zero;

        animator.SetBool("enMovimiento", false);
        animator.SetBool("corriendo", false);
        animator.SetBool("ataque1_JefeGor", false);
        animator.SetBool("ataque2_JefeGor", false);
        animator.SetBool("ataqueEsp_JefeGor", false);
        animator.SetBool("recibeDanio", true);

        if (!enFase2 && vida <= vidaMaxima / 2)
        {
            ActivarFase2();
        }

        if (vida <= 0)
        {
            muerto = true;
            Invoke("EmilinarCuerpo", 4f);
        }
        else
        {
            Vector2 rebote = new Vector2(direccion.x, 0).normalized;
            rb.AddForce(rebote * fuerzaRebotePersonalizada, ForceMode2D.Impulse);
        }
    }


    public void DesactivaDanio()
    {
        recibeDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void ActivarFase2()
    {
        enFase2 = true;

        // Aumenta tamaño
        transform.localScale *= multiplicadorTamano;

        // Aumenta daño
        danio += bonusDanioFase2;

        // Aumenta velocidad
        speed += bonusVelocidadFase2;

        Debug.Log("FASE 2 ACTIVADA");
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
        if (gorgonaPrefab1 != null && spawn1 != null)
            Instantiate(gorgonaPrefab1, spawn1.position, Quaternion.identity);

        if (gorgonaPrefab2 != null && spawn2 != null)
            Instantiate(gorgonaPrefab2, spawn2.position, Quaternion.identity);

        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ataqueRadios);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadious);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pretrificarRadios);
    }
}
