using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausar_Juego : MonoBehaviour
{
    public GameObject MenuPause;
    private bool juegoPausado = false;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }
    public void Reanudar()
    {
        MenuPause.SetActive(false);
        Time.timeScale = 1;
        juegoPausado=false;
    }
    public void Pausar()
    {
        MenuPause.SetActive(true);
        Time.timeScale = 0;
        juegoPausado=true;
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo el Juego ...");
        Application.Quit();
    }
}
