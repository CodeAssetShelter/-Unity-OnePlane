using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallExplosion : MonoBehaviour
{
    public AudioClip soundExplosion;

    public void PlayExplosionSound()
    {
        SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundExplosion);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
