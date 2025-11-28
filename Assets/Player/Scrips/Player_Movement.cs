using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player_Movement : MonoBehaviour
{
    public PlayerSoundController soundController;
    private IMovementStrategy moveStrategy;

    private ICommand attackCommand;
    private ICommand attack02Command;
    private ICommand defendCommand;

    private bool petrificado = false;
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
    public bool recibiendoDanio;
    private bool atacando;
    private bool atacando_02;
    public bool muerto;
    public bool defensa;
    private Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        moveStrategy = new NormalMovement();

        attackCommand = new AttackCommand();
        attack02Command = new Attack02Command();
        defendCommand = new DefendCommand();

        rb = GetComponent<Rigidbody2D>();
        vidaActual = GameManager.Instance.vidaJugador;
        sr = GetComponent<SpriteRenderer>();
        colorOriginal = sr.color;
    }

    void Update()
    {

        if(! muerto)
        {
            
            if (!atacando && !atacando_02 && !defensa && !petrificado)
            {

                moveStrategy.Move(this);
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capSuelo);
                enSuelo = hit.collider != null;

                if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDanio && !petrificado)
                {
                    soundController.playSaltar();
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }

        

            if (Input.GetKeyDown(KeyCode.K) && !petrificado) attackCommand.Execute(this);
            if (Input.GetKeyDown(KeyCode.L) && !petrificado) attack02Command.Execute(this);
            if (Input.GetKeyDown(KeyCode.J) && !petrificado) defendCommand.Execute(this);

        }


        Animaciones();

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
            soundController.playRecibirDanio();
            recibiendoDanio = true;
            vidaActual -= cantDanio;

            if (vidaActual <= 0)
            {
                muerto = true;

                StartCoroutine(RetrasoGameOver(3f));
            }

            if (!muerto)
            {
                // Rebote real alejando al jugador del enemigo
                Vector2 direccionGolpe = (transform.position - (Vector3)direccion).normalized;
                rb.AddForce(direccionGolpe * fuerzaRebote, ForceMode2D.Impulse);

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

        DesactivaAtaque();
        DesactivaAtaque_02();


        // iniciar rutina de recuperación
        StartCoroutine(RecuperarPetrificacion(duracion));
    }
    private IEnumerator RecuperarPetrificacion(float duracion)
    {
        yield return new WaitForSeconds(duracion);

        petrificado = false;

        // volver a color normal
        sr.color = colorOriginal;

    }


    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        //rb.linearVelocity = Vector2.zero;
    }

    public void Atacando()
    {   
        soundController.playAtaque1();
        atacando = true;
      
    }

    public void DesactivaAtaque()
    {
        atacando = false;
    }

    public void Atacando_02()
    {
       soundController.playAtaque2();
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
            GameManager.Instance.GameOver(2f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    }
