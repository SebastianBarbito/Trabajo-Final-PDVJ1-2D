using UnityEngine;

public class NormalMovement : IMovementStrategy
{
    public void Move(Player_Movement player)
    {
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * player.velocidad;

        player.animator.SetFloat("Movement", velocidadX * player.velocidad);

        if (velocidadX < 0)
            player.transform.localScale = new Vector3(-1, 1, 1);
        if (velocidadX > 0)
            player.transform.localScale = new Vector3(1, 1, 1);

        if (!player.recibiendoDanio)
        {
            Vector3 pos = player.transform.position;
            player.transform.position = new Vector3(pos.x + velocidadX, pos.y, pos.z);
        }
    }
}

