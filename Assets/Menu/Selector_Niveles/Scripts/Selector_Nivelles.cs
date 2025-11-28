using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Selector_Nivelles : MonoBehaviour
{

    public void Nivel_1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel_1");
    }

    public void Nivel_2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel_2");
    }
}
