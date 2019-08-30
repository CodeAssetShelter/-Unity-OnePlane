using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.tag == "Missile")
        {
            //GameManager.Instance.ScoreAdd("Missile");
            PublicValueStorage.Instance.AddMissileScore();
        }
    }
}
