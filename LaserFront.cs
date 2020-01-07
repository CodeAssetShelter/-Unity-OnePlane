using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFront : MonoBehaviour
{
    private LaserVer2 parentScript;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;

    private bool createdLaser = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        parentScript = this.transform.parent.GetComponent<LaserVer2>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parentScript.tag != "PlayerReflectLaser")
        {
            if (collision.tag == "PlayerShield")
            {
                parentScript.activeLaserFront = false;
                if (parentScript.tag == "NormalLaser" && createdLaser == false)
                {
                    // do Create Laser for player
                    //Debug.Log("닿았다");
                    parentScript.createLaserFrontReady = true;
                    createdLaser = true;
                }

                if (parentScript.tag == "LaserNo5Laser" && createdLaser == false)
                {
                    // do Create Laser for player
                    parentScript.createLaserFrontReady = true;
                    createdLaser = true;
                }
            }

            if (collision.tag == "Player")
            {
                //Debug.Log("LaserFront reached to player");
                parentScript.activeLaserFront = false;
                //Destroy(parentScript.gameObject);                
            }
        }
        if (collision.tag == "WasteBasket")
        {
            parentScript.activeLaserFront = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ((parentScript.tag.Contains("Player") == false)))
        {
            PublicValueStorage.Instance.GetPlayerComponent().ActivatePlayerDie();
        }
        if (parentScript.laserType == LaserVer2.LaserType.CircleLaser)
        {
            if (collision.tag == "PlayerShield")
            {
                parentScript.activeCircleLaser = false;

                PublicValueStorage.Instance.AddMissileScore();

                float value = Time.deltaTime * 1.2f;
                parentScript.transform.localScale -= new Vector3(value, value, 0);

                if (parentScript.transform.localScale.x <= 1.0f)
                {
                    Vector3 reflectPosition = collision.transform.position;
                    reflectPosition.y += spriteRenderer.bounds.extents.y * 5.0f;

                    parentScript.CreateLaserForPlayer(reflectPosition);
                    Destroy(parentScript.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerShield" || collision.tag == "Player")
        {
            if (parentScript.laserType == LaserVer2.LaserType.CircleLaser)
            {
                parentScript.activeCircleLaser = true;
            }
            else
            {
                parentScript.activeLaserFront = true;
            }
        }
    }
}