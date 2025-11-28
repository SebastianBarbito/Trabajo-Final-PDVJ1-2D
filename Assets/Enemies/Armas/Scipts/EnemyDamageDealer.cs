using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{

    public int damage = 1;           // Daño del arma
    public Transform owner;          // El enemigo dueño de esta arma

    private void Awake()
    {
        // Toma el dueño automáticamente si no está asignado
        if (owner == null)
            owner = GetComponentInParent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player_Movement player = collision.GetComponent<Player_Movement>();
        if (player == null) return;
        player.RecibeDanio(owner.position, damage);
    }
}
