using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour
{
    public AudioClip[] soundExplosion;
    public AudioClip soundBigExplosion;

    public void PlayExplosionSound()
    {
        int index = Random.Range(0, soundExplosion.Length);
        SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundExplosion[index]);
    }

    public void PlayBigExplosionSound()
    {
        SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundBigExplosion);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
