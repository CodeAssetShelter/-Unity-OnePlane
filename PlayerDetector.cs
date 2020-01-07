using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private GameObject player;

    public GameObject GetPlayer()
    {
        if (player != null)
        {
            Debug.Log("Return Player");
            return player.transform.parent.gameObject;
        }
        else
        {
            Debug.Log("Return Null");
            return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision.tag : " + collision.tag);
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Found Player! ");
            player = collision.gameObject;
            //this.gameObject.SetActive(false);
            //this.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("destroy");
    }
}
