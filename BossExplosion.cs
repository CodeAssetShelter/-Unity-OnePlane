using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExplosion : MonoBehaviour
{
    public AudioClip explosionSound;


    public void ExplosionSoundPlay()
    {
        SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, explosionSound);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
