using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("El Player se callo al vacio por noob");

            collision.GetComponent<Player_Movement>().RecibeDanio(Vector2.zero,99);
        }
    }
}
