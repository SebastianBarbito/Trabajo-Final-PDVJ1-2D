using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Selector_Nivelles : MonoBehaviour
{
    public GameObject nivelBottonPrefab;
    public Transform buttonContainer;
    public int totalLevels = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateLevelButton();
    }

    void GenerateLevelButton()
    {
        for (int i = 1; i < totalLevels; i++)
        {
            GameObject buttonObjt = Instantiate(nivelBottonPrefab, buttonContainer);
            buttonObjt.GetComponentInChildren<TextMeshProUGUI>().text = "Nivel " + 1;

            int levelIndex = i;
            buttonObjt.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Nivel_" + levelIndex);
            }
            );
        }
    }
}
