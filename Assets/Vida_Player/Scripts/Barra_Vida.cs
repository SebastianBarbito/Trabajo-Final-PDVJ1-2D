using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Barra_Vida : MonoBehaviour
{
    public Image rellenoBarraVida;
    //private Player_Movement playerController;
    private float vidaMaxima;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerController = GameObject.Find("Player").GetComponent<Player_Movement>();
        //vidaMaxima = Player_Movement.vidaActual;
    }

    // Update is called once per frame
    void Update()
    {
        //rellenoBarraVida.fillAmount = playerController.vida / vidaMaxima;
        float vidaActual = Player_Movement.vidaActual;
        float vidaMaxima = Player_Movement.MAX_VIDA;

        if (vidaMaxima > 0)
        {
            // 3. ¡El cálculo correcto!
            rellenoBarraVida.fillAmount = vidaActual / vidaMaxima;
        }
    }

}
