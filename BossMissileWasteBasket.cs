using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissileWasteBasket : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "WasteBasket")
        {
            Destroy(this.gameObject);
        }
    }
}
