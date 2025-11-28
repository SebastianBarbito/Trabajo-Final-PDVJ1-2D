using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    public GameObject levelCompletedPanel;

    public AudioClip audioGameOver;

    public AudioClip audioLevelCompleted;

    private AudioSource audioSource;


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
        audioSource = GetComponent<AudioSource>();

        if (levelCompletedPanel != null)
            levelCompletedPanel.SetActive(false);


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
    public void GameOver(float delay = 2f)
    {
        if (gameOverActivo) return;
        gameOverActivo = true;

        StartCoroutine(GameOverRoutine(delay));
    }

    private IEnumerator GameOverRoutine(float delay)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        MusicController.Instance.StopMusic();

        if (audioSource != null && audioGameOver != null)
            audioSource.PlayOneShot(audioGameOver);

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(delay);
    }

    public void LevelCompleted(float delay = 2f)
    {
        StartCoroutine(LevelCompletedRoutine(delay));
    }
    private IEnumerator LevelCompletedRoutine(float delay)
    {
        // Mostrar UI
        if (levelCompletedPanel != null)
            levelCompletedPanel.SetActive(true);
        MusicController.Instance.StopMusic();

        // Reproducir sonido
        if (audioSource != null && audioLevelCompleted != null)
            audioSource.PlayOneShot(audioLevelCompleted);

        Time.timeScale = 0f;

        // Espera sin congelar el audio
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1f;

        // Pasar al siguiente nivel
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


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
