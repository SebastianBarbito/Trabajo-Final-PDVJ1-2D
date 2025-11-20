using UnityEngine;

public class GorgonaPetrifyAttack : MonoBehaviour
{
    public Transform owner;            // referencia al enemigo
    public float petrifyDuration = 3f; // cuánto tiempo queda petrificado
    public int damageOnHit = 0;        // daño opcional (si no querés daño, dejá 0)

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        Player_Movement player = col.GetComponent<Player_Movement>();
        if (player == null) return;

        // Dirección desde el enemigo hacia el jugador
        Vector2 direccion = (player.transform.position - owner.position).normalized;

        // Aplicar daño opcional
        if (damageOnHit > 0)
            player.RecibeDanio(direccion, damageOnHit);

        // Aplicar petrificación
        player.ActivarPetrificacion(petrifyDuration);
    }
}
