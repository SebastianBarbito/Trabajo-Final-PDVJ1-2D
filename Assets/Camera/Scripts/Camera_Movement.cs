using System.Security.Cryptography;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    public Transform objetivo;
    public float velocidadCamara = 2.5f;
    public Vector3 desplazamiento;

    private void LateUpdate()
    {
        Vector3 posicionDeseada = objetivo.position + desplazamiento;

        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara);

        transform.position = posicionSuavizada;
    }
}
