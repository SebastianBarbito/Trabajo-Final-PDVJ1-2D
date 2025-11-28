using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoSaltar;
    public AudioClip sonidoataque1;
    public AudioClip sonidoataque2;
    public AudioClip recibirDanio;
   
    public void playSaltar()
    {
      audioSource.PlayOneShot(sonidoSaltar);
    }
    public void playAtaque1()
    {
      audioSource.PlayOneShot(sonidoataque1);
    }
    public void playAtaque2()
    {
      audioSource.PlayOneShot(sonidoataque2);
    }
    public void playRecibirDanio()
    {
      audioSource.PlayOneShot(recibirDanio);
    }
}
