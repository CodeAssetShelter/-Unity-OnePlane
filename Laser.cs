using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Laser : MonoBehaviour
{
    public GameObject laser;
    public GameObject line;
    public GameObject start;
    public GameObject end;
    private Vector2 endInitPosition = Vector2.zero;


    private LineRenderer lineRenderer;
    private BoxCollider2D lineCollider;
    private SpriteRenderer startPoint;
    private SpriteRenderer endPoint;

    private float lineWidthStart = 1.0f;
    private float lineWidthEnd = 1.0f;

    private Vector2 playerposition = new Vector2(0, -4.1f);
    private Vector2 direction = Vector2.down;

    public OPCurves opCurves;

    // Laser launch triggers
    [Header("- Common Laser values")]
    public bool activeLaser = false;
    public float laserEndPointDelay = 2.0f;
    private float angle;
    private float playerLaserAngle;
    private bool playerLaserMovable;


    private bool isThisPlayerLaser = false;
    public bool activateEndLaser = false;
    public bool activateStartLaser = true;
    public bool activateLaserControl = false;

    // StartLaser Timer for LaserNo5 Mode
    private float startLaserTimer = 0;
    private float startLaserTimerCheck = 0;

    // EndLaser timer for RapidLaser Mode
    public float endLaserTimerMin = 0.2f;
    private float endLaserTimerCheck = 0;
    private float endLaserTimer = 0;
    private float speed = 1.0f;

    // Player's Laser Values
    [Header("- Player's Laser values")]
    public float laserRotateSpeed = 2.0f;
    private Vector2 mouseInitPosition;
    

    private void OnEnable()
    {
        endInitPosition = end.transform.position;
        playerposition = PublicValueStorage.Instance.GetPlayerPos();
        speed = PublicValueStorage.Instance.GetAddSpeedRivisionValue();

        //InitLaserVectorOnEnable();
    }

    // Update is called once per frame
    void Update()
    {
        if (isThisPlayerLaser == true)
        {
            if (activateLaserControl == true)
                LaserControlledByPlayer();
        }
        if (activeLaser == true)
        {
            // Player's Laser Mode
            if (isThisPlayerLaser == true)
            {
                if (activateStartLaser == true)
                {
                    if (start.transform.localPosition.y < 20.0f)
                    {
                        start.transform.Translate(Vector2.up * Time.deltaTime * 5.0f * speed, Space.Self);
                    }
                }
                if (activateEndLaser == true)
                {
                    end.transform.Translate(Vector2.up * Time.deltaTime * 5.0f * speed, Space.Self);
                }
            }
            // Enemy's Laser Mode
            else
            {
                // StartEndLaserTimer is activate only Enemy's Laser Mode
                StartLaserTimer();

                if (activateStartLaser == true)
                {
                    start.transform.Translate(direction * Time.deltaTime * 5.0f * speed, Space.World);
                }
                if (activateEndLaser == true)
                {
                    end.transform.Translate(direction * Time.deltaTime * 5.0f * speed , Space.World);
                }

            }
            ActivateLaser();
        }
        activateStartLaser = true;
    }


    public void SetStartEndPointTimer (float endTime)
    {        
        endLaserTimer = Random.Range(endLaserTimerMin, endTime);
    }

    public void SetLaserNo5ModeTimer (float endTime, float emergencyBreakTime)
    {
        startLaserTimer = emergencyBreakTime;
        endLaserTimer = endTime;
    }


    private void StartLaserTimer()
    {
        if (endLaserTimerCheck >= endLaserTimer)
        {
            activateEndLaser = true;
            return;
        }
        endLaserTimerCheck += Time.deltaTime;
    }


    public void InitLaserVector(string tag, float speed = 1.0f)
    {
        this.tag = tag;
        this.speed = speed;

        isThisPlayerLaser = (this.tag == "PlayerReflectLaser") ? true : false;
        activateLaserControl = isThisPlayerLaser;

        end.transform.position = start.transform.position = this.transform.position;

        lineRenderer = line.GetComponent<LineRenderer>();
        lineCollider = line.GetComponent<BoxCollider2D>();
        startPoint = start.GetComponent<SpriteRenderer>();
        endPoint = end.GetComponent<SpriteRenderer>();

        if (this.tag == "PlayerReflectLaser" || isThisPlayerLaser == true)
        {
            this.tag = line.tag = startPoint.tag = endPoint.tag = lineCollider.tag = "PlayerReflectLaser";
            startPoint.GetComponent<LaserStart>().isTouchPlayer = isThisPlayerLaser;

            direction = Vector2.up;
            Vector2 upVector = playerposition;
            upVector.y += 1.0f;
            angle = Quaternion.FromToRotation(Vector3.up, (Vector3)upVector - (Vector3)playerposition).eulerAngles.z;
        }
        else
        {
            //Debug.Log(this.name + " || tag : " + this.tag + " || " + isThisPlayerLaser);

            direction = opCurves.SeekDirection(endPoint.transform.position, playerposition);
            angle = Quaternion.FromToRotation(Vector3.up, endPoint.transform.position - (Vector3)playerposition).eulerAngles.z;

        }



        lineWidthStart = startPoint.bounds.extents.x * 2.0f;
        lineWidthEnd = endPoint.bounds.extents.x * 2.0f;
        lineRenderer.startWidth = lineWidthStart;
        lineRenderer.endWidth = lineWidthEnd;

        lineCollider.transform.position = startPoint.transform.position;
        lineCollider.size = new Vector2(lineWidthStart, startPoint.bounds.extents.y * 2.0f);


        activeLaser = true;

        lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);
        startPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
        endPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void InitLaserVectorOnEnable()
    {
        isThisPlayerLaser = (this.tag == "PlayerReflectLaser") ? true : false;

        end.transform.position = start.transform.position = this.transform.position;

        lineRenderer = line.GetComponent<LineRenderer>();
        lineCollider = line.GetComponent<BoxCollider2D>();
        startPoint = start.GetComponent<SpriteRenderer>();
        endPoint = end.GetComponent<SpriteRenderer>();

        if (this.tag == "PlayerReflectLaser" || isThisPlayerLaser == true)
        {
            this.tag = line.tag = startPoint.tag = endPoint.tag = lineCollider.tag = "PlayerReflectLaser";
            startPoint.GetComponent<LaserStart>().isTouchPlayer = isThisPlayerLaser;

            direction = Vector2.up;
            Vector2 upVector = playerposition;
            upVector.y += 1.0f;
            angle = Quaternion.FromToRotation(Vector3.up, (Vector3)upVector - (Vector3)playerposition).eulerAngles.z;
        }
        else
        {
            //Debug.Log(this.name + " || tag : " + this.tag + " || " + isThisPlayerLaser);

            direction = opCurves.SeekDirection(endPoint.transform.position, playerposition);
            angle = Quaternion.FromToRotation(Vector3.up, endPoint.transform.position - (Vector3)playerposition).eulerAngles.z;

        }


        lineWidthStart = startPoint.bounds.extents.x * 2.0f;
        lineWidthEnd = endPoint.bounds.extents.x * 2.0f;
        lineRenderer.startWidth = lineWidthStart;
        lineRenderer.endWidth = lineWidthEnd;

        lineCollider.transform.position = startPoint.transform.position;
        lineCollider.size = new Vector2(lineWidthStart, startPoint.bounds.extents.y * 2.0f);


        activeLaser = true; 

        lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);
        startPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
        endPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    // 190701 LifeBalance
    // Temp values

    //float currTemp = 0;
    //float currSpeed = 50.0f;
    //float currRes = 0;
    //Quaternion prevRot;
    //float laserz = 0;
    private void LaserControlledByPlayer()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    mouseInitPosition = Input.mousePosition;
        //}
        //if (Input.GetMouseButtonUp(0))
        //{
            
        //}

        if ((Input.touchCount >= 1 || Input.GetMouseButton(0)) &&
            (EventSystem.current.IsPointerOverGameObject() == false &&
            EventSystem.current.IsPointerOverGameObject(0) == false))
        {
            playerLaserAngle = Quaternion.FromToRotation(Vector3.down, Camera.main.WorldToScreenPoint(endInitPosition) - Input.mousePosition).eulerAngles.z;

            // 190701 LifeBalance
            // Laser movement immediately
            //Debug.Log(PlayerLaserAngle);
            if (playerLaserAngle < 45f || (playerLaserAngle > 360f - 45f))
                this.transform.rotation = Quaternion.Euler(0, 0, playerLaserAngle);





            // 190701 LifeBalance
            // temp laser movement slowly

            //laserz = Quaternion.FromToRotation(Vector3.down, Camera.main.WorldToScreenPoint(start.transform.position) - Camera.main.WorldToScreenPoint(end.transform.position)).eulerAngles.z;

            //if (laserz >= PlayerLaserAngle - 1.0f && laserz <= PlayerLaserAngle + 1.0f)
            //{
            //}
            //else
            //{
            //    if (laserz < PlayerLaserAngle && 180f + 40f > laserz)
            //    {
            //        this.transform.Rotate(0, 0, this.transform.rotation.z + (currSpeed * Time.deltaTime));
            //    }
            //    if (laserz > PlayerLaserAngle && 180f - 40f < laserz)
            //    {
            //        this.transform.Rotate(0, 0, this.transform.rotation.z - (currSpeed * Time.deltaTime));
            //    }
            //}
        }
    }

    private void ActivateLaser()
    {
        float size = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
        lineCollider.size = new Vector2(size + lineCollider.size.y, lineCollider.size.y);

        angle = Quaternion.FromToRotation(Vector3.up, endPoint.transform.position - startPoint.transform.position).eulerAngles.z;
        lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);

        lineCollider.transform.position = (startPoint.transform.position + endPoint.transform.position) * 0.5f;

        lineRenderer.SetPosition(0, startPoint.transform.position);
        lineRenderer.SetPosition(1, endPoint.transform.position);
    }

    public void activateLaser(bool active)
    {
        activeLaser = active;
    }

}
