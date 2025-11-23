using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour  
{
    public static GameManager Instance;
    public int vidaJugador = 7;
    public int vidaMaxima = 7;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button reiniciarButton;
    public Button menuButton;

    private bool gameOverActivo = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverPanel != null)
        
             gameOverPanel.SetActive(false);

        if (reiniciarButton != null)

            reiniciarButton.onClick.AddListener(ReiniciarEscena);

        if (menuButton != null)

            menuButton.onClick.AddListener(IrAlMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOverActivo)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReiniciarEscena();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                IrAlMenu();
            }
        }
    }
    public void GameOver()
    {
        if (gameOverActivo) return;
        gameOverActivo = true;

        if (gameOverPanel != null)
        { 
            gameOverPanel.SetActive(true);
        }

    }
    public void ReiniciarEscena()
    {
        Player_Movement.vidaActual = Player_Movement.MAX_VIDA;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Player_Movement.vidaActual = Player_Movement.MAX_VIDA;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
