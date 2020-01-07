using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
    public AudioClip[] vansaSori = new AudioClip[4];
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Item")
        {
            int rand = Random.Range(0, vansaSori.Length);
            SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, vansaSori[rand]);
        }
    }
}
