using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float velocidad = 5f;
    public float vida = 3;

    public float fuerzaSalto = 10f;
    public float fuerzaRebote = 5f;
    public float longitudRaycast = 1.0f;
    public LayerMask capSuelo;

    private bool enSuelo;
    private bool recibiendoDanio;
    private bool atacando;
    private bool atacando_02;
    public bool muerto;
    private Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if(! muerto)
        {

            if (!atacando || atacando_02)
            {
                Movimiento();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capSuelo);
                enSuelo = hit.collider != null;

                if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDanio)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }

            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                Atacando();
            }

            if (Input.GetKeyDown(KeyCode.K) && !atacando_02 && enSuelo)
            {
                Atacando();
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                Atacando_02();
            }
            if (Input.GetKeyDown(KeyCode.L) && !atacando && enSuelo)
            {
                Atacando_02();
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
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hacha"))
        {
            float fuerzaExtra = 2f; //  más alto = más empuje
            Vector2 direccion = new Vector2(collision.gameObject.transform.position.x, 0);
            fuerzaRebote *= fuerzaExtra;
            RecibeDanio(direccion, 2);
            fuerzaRebote /= fuerzaExtra; //  vuelve al valor normal
        }

    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if(!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            if (vida <=0)
            {
                muerto = true;
            }
            if (!muerto)
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

    public void Atacando()
    {
        atacando = true;
    }

    public void DesactivaAtaque()
    {
        atacando = false;
    }

    public void Atacando_02()
    {
        atacando_02 = true;
    }

    public void DesactivaAtaque_02()
    {
        atacando_02 = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    }
