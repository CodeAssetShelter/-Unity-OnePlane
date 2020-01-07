using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEndVer2 : MonoBehaviour
{
    private LaserVer2 parentScript;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        parentScript = this.transform.parent.GetComponent<LaserVer2>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.tag != "PlayerReflectLaser")
        {
            if (collision.tag == "PlayerShield")
            {
                if (parentScript.createLaserFrontReady == true && parentScript.createLaserEndReady == false)
                {
                    parentScript.createLaserEndReady = true;

                    Vector3 reflectPosition = collision.transform.position;
                    reflectPosition.y += spriteRenderer.bounds.extents.y * 5.0f;

                    parentScript.CreateLaserForPlayer(reflectPosition);
                }
                if (parentScript.createLaserFrontReady != false)
                {
                    Destroy(parentScript.gameObject);
                }
            }
            if (collision.tag == "Player")
            {
                Destroy(parentScript.gameObject);
            }
            if (collision.tag == "WasteBasket")
            {
                Destroy(parentScript.gameObject);
            }
        }
        else
        {
            if (collision.tag == "WasteBasket")
            {
                Destroy(parentScript.gameObject);
            }
            //Debug.Log(this.GetType().ToString() + " detected " + collision.tag);
        }
    }
}
