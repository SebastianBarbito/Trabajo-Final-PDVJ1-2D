using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
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

}
