using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Nivel_1");
    }
    public void Salir()
    {
        Debug.Log("Saliendo el Juego ...");
        Application.Quit();
    }
    public void Niveles()
    {
        SceneManager.LoadScene("Selector_Niveles");
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }

}
