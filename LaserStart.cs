using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserStart : MonoBehaviour
{
    public new BoxCollider2D collider;
    public new SpriteRenderer renderer;


    public Laser myParent;

    [HideInInspector]
    public bool isTouchPlayer = false;
    [HideInInspector]
    public bool isRealLaser = false;

    private void OnEnable()
    {
        collider.size = new Vector2(renderer.bounds.extents.x * 2.0f, renderer.bounds.extents.y * 2.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.tag != "PlayerReflectLaser")
        {
            if (collision.tag == "Player")
            {
                Destroy(this.transform.parent);
            }
            if (collision.tag == "PlayerShield")
            {
                if (this.tag == "ReflectLaser")
                {
                    // Player shot Laser
                    if (isTouchPlayer == false)
                    {
                        isTouchPlayer = true;
                        myParent.activateStartLaser = false;
                        //myParent.activateEndLaser = true;
                        GameObject newLaser = Instantiate(this.transform.parent.gameObject, this.transform.position, Quaternion.Euler(0, 0, 0));
                        Laser laserScript = newLaser.GetComponent<Laser>();
                        laserScript.activateStartLaser = true;
                        laserScript.activateEndLaser = false;
                        laserScript.activateLaserControl = true;
                        laserScript.InitLaserVector("PlayerReflectLaser");
                    }
                }

                // Do not create new Laser for Player
                // Do not Set true to Activate Start Laser
                if (this.tag == "RapidLaser")
                {
                    if (isTouchPlayer == false)
                    {
                        isTouchPlayer = true;
                        myParent.activateStartLaser = false;
                    }
                }
            }
        }
        if (collision.tag == "WasteBasket")
        {
            myParent.activateStartLaser = false;
            myParent.activateEndLaser = true;
            //myParent.activateLaserControl = false;
        }
    }
}
