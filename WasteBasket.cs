using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBasket : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("WASTE : " + collision.tag);
        if (collision.tag.Contains("Laser") == true)
        {
            //Debug.Log(collision.tag + " is not destroy target");
            return;
        }
        if (collision.tag != "Enemy" && collision.tag != "BOSS")
        {
            Destroy(collision.gameObject);
        }
    }
}
