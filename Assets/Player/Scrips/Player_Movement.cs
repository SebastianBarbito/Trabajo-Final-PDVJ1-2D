using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player_Movement : MonoBehaviour
{
    private bool petrificado = false;
    public bool puedeMover = true;
    public bool puedeAtacar = true;
    private SpriteRenderer sr;
    private Color colorOriginal;


    public float velocidad = 5f;

    public const float MAX_VIDA = 7f;
    public static float vidaActual = MAX_VIDA;

    public float fuerzaSalto = 10f;
    public float fuerzaRebote = 5f;
    public float knockbackDuration = 0.2f;
    public float longitudRaycast = 1.0f;
    public LayerMask capSuelo;

    private bool enSuelo;
    private bool recibiendoDanio;
    private bool atacando;
    private bool atacando_02;
    public bool muerto;
    public bool defensa;
    private Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = GameManager.Instance.vidaJugador;
        sr = GetComponent<SpriteRenderer>();
        colorOriginal = sr.color;
    }

    void Update()
    {

        if(! muerto)
        {
            
            if (!atacando && !atacando_02 && !defensa)
            {
                if (!puedeMover || petrificado)
                    return;

                Movimiento();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capSuelo);
                enSuelo = hit.collider != null;

                if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDanio)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }

            }

            if (Input.GetKeyDown(KeyCode.K) && !atacando && enSuelo)
            {
                Atacando();
            }

            
            if (Input.GetKeyDown(KeyCode.L) && !atacando_02 && enSuelo)
            {
                Atacando_02();
            }
            if (Input.GetKeyDown(KeyCode.J) && !defensa && enSuelo)
            {
                Defensa();
            }
        }
        

        Animaciones();

    }

    public void Movimiento()
    {
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

        animator.SetFloat("Movement", velocidadX * velocidad);

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Vector3 posicion = transform.position;

        if (!recibiendoDanio)

            transform.position = new Vector3(posicion.x + velocidadX, posicion.y, posicion.z);
    }

    public void Animaciones()
    {
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("muerto", muerto);
        animator.SetBool("Atacando_02", atacando_02);
        animator.SetBool("Defensa", defensa);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            Minotaur_Jefe enemigoJefe = collision.GetComponentInParent<Minotaur_Jefe>();
            if (enemigoJefe != null)
                HandleCollisionDamage(collision.gameObject.transform.position, enemigoJefe.danioHacha);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (defensa)
        {
            Debug.Log("¡Defensa Exitosa! Aplicando rebote.");
            recibiendoDanio = false;
            // 1. Aplica la fuerza de rebote
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0).normalized;
            float fuerzaReboteDefensa = fuerzaRebote * 2f;
            rb.AddForce(rebote * fuerzaReboteDefensa, ForceMode2D.Impulse);

            // 2. Bloquea el movimiento manual durante el rebote
            // Usamos la Coroutine para gestionar el tiempo de inmovilidad
            StartCoroutine(BlockMovement(knockbackDuration));
            return;
        }

        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vidaActual -= cantDanio;

            if (vidaActual <= 0)
            {
                muerto = true;
                
                StartCoroutine(RetrasoGameOver(3f));
            }

            if (!muerto)
            {
                Vector2 rebote = new Vector2(direccion.x * 2, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(BlockMovement(knockbackDuration));
            }
        }
    }


    IEnumerator BlockMovement(float duration)
    {
        // Bloquea el control de movimiento y la aplicación de daño
        recibiendoDanio = true;

        // Espera el tiempo de duración del rebote
        yield return new WaitForSeconds(duration);

        // Libera el control y detiene cualquier velocidad residual
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }
    public void ActivarPetrificacion(float duracion)
    {
        if (petrificado) return;

        petrificado = true;

        // Cambiar apariencia a gris piedra
        sr.color = new Color(0.6f, 0.6f, 0.6f, 1f);

        // congelar movimiento y animaciones
        rb.linearVelocity = Vector2.zero;

        // bloquear acciones
        puedeMover = true;
        puedeAtacar = true;

        // iniciar rutina de recuperación
        StartCoroutine(RecuperarPetrificacion(duracion));
    }
    private IEnumerator RecuperarPetrificacion(float duracion)
    {
        yield return new WaitForSeconds(duracion);

        petrificado = false;

        // volver a color normal
        sr.color = colorOriginal;

        // habilitar movimiento y ataque
        puedeMover = true;
        puedeAtacar = true;
    }


    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        //rb.linearVelocity = Vector2.zero;
    }

    public void Atacando()
    {
        if (!puedeAtacar || petrificado)
            return;
        atacando = true;
    }

    public void DesactivaAtaque()
    {
        atacando = false;
    }

    public void Atacando_02()
    {
        if (!puedeAtacar || petrificado)
            return;

        atacando_02 = true;
    }

    public void DesactivaAtaque_02()
    {
        atacando_02 = false;
    }

    public void Defensa()
    {
        defensa = true;
        DesactivaDanio();
    }

    public void DesactivaDefensa()
    {
        defensa = false;
    }

    private void HandleCollisionDamage(Vector3 collisionPosition, int damageAmount)
    {
        if (defensa)
        {
            // Si hay defensa, el RecibeDanio() se encargará del rebote y de NO aplicar daño.
            // No salimos con 'return' aquí, sino que pasamos el control a RecibeDanio.
        }

        float fuerzaExtra = 2f; // Puedes usar esta variable para modificar la fuerza de rebote

        Vector2 direccion = new Vector2(collisionPosition.x, 0);

        fuerzaRebote *= fuerzaExtra;
        // rebote por defensa O daño por golpe.
        RecibeDanio(direccion, damageAmount);
        // Volvemos al valor normal
        fuerzaRebote /= fuerzaExtra;
    }

    private IEnumerator RetrasoGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    }
