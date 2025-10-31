using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float velocidad = 5f;

    public Animator animator;

    void Start()
    {
        
    }

    void Update()
    {
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

        animator.SetFloat("Movement", velocidadX * velocidad);

        if(velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if(velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Vector3 posicion = transform.position;

        transform.position = new Vector3(posicion.x + velocidadX, posicion.y, posicion.z);
    }
}
