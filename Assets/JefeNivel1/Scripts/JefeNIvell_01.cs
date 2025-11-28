using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class JefeNIvell_01 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.LevelCompleted(3.5f);
        }
    }
}
