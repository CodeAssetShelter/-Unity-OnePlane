using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLIMissile : MonoBehaviour
{
    private Vector2[] routes = new Vector2[2];
    public float moveSpeed = 2.0f;
    private float moveProcess = 0f;
    private bool onLadderRoute = false;
    private Vector3 direction = Vector3.down;

    [SerializeField]
    private bool canIGoNow = false;
    private GameObject metObject = null;

    private MissileModuleRLI rliParent = null;
    // Update is called once per frame
    void Update()
    {
        if (canIGoNow == true)
        {
            MissileMove();
        }
    }

    private void MissileMove()
    {
        if (onLadderRoute == false)
        {
            this.transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            return;
        }
        if (moveProcess >= 1.0f)
        {
            onLadderRoute = false;
            this.transform.position = routes[1];
            moveProcess = 0;
            return;
        }
        moveProcess += moveSpeed * Time.deltaTime;
        this.transform.position = Vector2.Lerp(routes[0], routes[1], moveProcess);
    }

    public void SetMissileInfo(Vector3 startPos, Vector3 direction, float PVCRivision = 1.0f)
    {
        float rivisionMoveSpeed = moveSpeed * PVCRivision;
        if (rivisionMoveSpeed >= 3.0f)
        {
            moveSpeed = 3.0f;
        }
        else
        {
            moveSpeed = rivisionMoveSpeed;
        }

        this.transform.position = startPos;
        this.direction = direction;

        canIGoNow = true;
    }

    public void SetParentComponent(MissileModuleRLI parentScript)
    {
        rliParent = parentScript;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("tag : " + collision.name + " //// " + metObject);
        if (metObject == collision.transform.parent.gameObject)
        {
            //Debug.Log("Same");
        }
        else
        {
            onLadderRoute = false;
            moveProcess = 0;
            this.transform.position = routes[0];
        }

        if (collision.tag == "RLIRoute" &&
            onLadderRoute == false && 
            metObject != collision.transform)
        {
            LadderRoute component = collision.transform.parent.parent.GetComponent<LadderRoute>();
            // Set position by normal order
            if (collision.name == "Top")
            {
                routes = component.GetStartEndPositions();
            }
            // Set position by reverse order
            if (collision.name == "Bottom")
            {
                routes = component.GetStartEndPositions();
                Vector2 temp = routes[0];
                routes[0] = routes[1];
                routes[1] = temp;                
            }

            metObject = collision.transform.parent.gameObject;
            onLadderRoute = true;
        }

        if (collision.tag == "Player" || collision.tag == "BOSS")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (rliParent != null)
        {
            Debug.Log("I'm Last bullet!");
            rliParent.DestroyLastBullet();
        }
    }
}
