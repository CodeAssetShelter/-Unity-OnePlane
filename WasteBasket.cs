using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBasket : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Enemy" && collision.tag != "BOSS")
            Destroy(collision.gameObject);
    }
}
