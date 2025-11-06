using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Barra_Vida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private Player_Movement playerController;
    private float vidaMaxima;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<Player_Movement>();
        vidaMaxima = playerController.vida;
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vida / vidaMaxima;
    }
}
