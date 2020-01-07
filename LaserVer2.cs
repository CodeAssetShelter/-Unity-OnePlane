using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserVer2 : MonoBehaviour
{
    // enum Pattern { Idle = 0, SpawnOption, NormalLaser, CircleLaser, ReflectLaser, LaserNo5, CoolDown, StateCount }
    // Common Data
    public enum LaserType { NormalLaser = 0, PlayerLaser, CircleLaser, ReflectLaser, LaserNo5, StateCount }
    public LaserType laserType;

    private Vector3 playerPos = Vector3.zero;
    private Vector3 startPos = Vector3.zero;

    public AudioClip soundPlayerLaser;

    [Header(" - Original Prefabs")]
    public LaserVer2 laserPrefab;

    [Header ("- Laser & Children")]
    public LaserVer2 laser;
    public LineRenderer line;
    private BoxCollider2D lineCollider;
    public LaserFront front;
    public LaserEndVer2 end;

    public OPCurves oPCurves;

    [Header("- Laser movement")]
    public bool activeLaserFront = false;
    public float laserFrontSpeed = 1.0f;
    private Vector3 laserFrontVector = Vector3.zero;

    public bool activeLaserEnd = false;
    public float laserEndSpeed = 1.0f;
    private Vector3 laserEndVector = Vector3.zero;

    private float laserEndTimer = 0;
    public float laserEndLaunchDelay = 1.0f;

    public bool activeCircleLaser = false;
    public float circleLaserScale = 2.0f;
    private bool circleLaserFullCharged = false;

    private Vector3 endInitPosition;

    public bool createLaserFrontReady = false;
    public bool createLaserEndReady = false;


    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        playerPos = PublicValueStorage.Instance.GetPlayerPos();
        startPos = this.transform.position;

        laser = this;
        //Debug.Log("Player : " + playerPos);

        line.SetPosition(0, this.transform.position);
        line.SetPosition(1, this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
    }
    
    private void MoveLaser()
    {
        if (laserType != LaserType.CircleLaser)
        {
            if (activeLaserFront == true)
            {
                front.transform.Translate(laserFrontVector * Time.deltaTime * laserFrontSpeed, Space.World);
                line.SetPosition(0, front.transform.position);
            }
            if (activeLaserEnd == true)
            {
                end.transform.Translate(laserEndVector * Time.deltaTime * laserEndSpeed, Space.World);
                line.SetPosition(1, end.transform.position);

                AdjustLineCollider();
            }

            if (laserEndTimer >= laserEndLaunchDelay)
            {
                activeLaserEnd = true;

                if (this.tag == "ReflectLaser")
                {
                    StopCoroutine("ControllerPlayerLaser");
                }
            }
            else
            {
                laserEndTimer += Time.deltaTime;
            }
        }
        if (laserType == LaserType.CircleLaser)
        {
            if (activeCircleLaser == true)
            {
                if (activeLaserFront == false)
                {

                    if (circleLaserFullCharged == false)
                    {
                        this.transform.Translate(laserFrontVector * Time.deltaTime * laserFrontSpeed * 0.1f, Space.World);
                        this.transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime, 0);
                    }
                    else
                    {
                        this.transform.Translate(laserFrontVector * Time.deltaTime * laserFrontSpeed, Space.World);
                    }

                    if (this.transform.localScale.x >= circleLaserScale)
                    {
                        activeLaserFront = true;
                        circleLaserFullCharged = true;
                    }
                }
                if (activeLaserFront == true)
                {
                    //Debug.Log("Move");
                    this.transform.Translate(laserFrontVector * Time.deltaTime * laserFrontSpeed, Space.World);
                    //line.SetPosition(0, front.transform.position);
                }
            }
        }
        //Debug.Log("Now Pattern : " + laserType);
    }


    // This will Adjust LineCollider's BoxCollider postision, rotation, size
    private void AdjustLineCollider()
    {
        if (lineCollider == null) return;
        float size = Vector2.Distance(front.transform.position, end.transform.position);
        lineCollider.size = new Vector2(size + lineCollider.size.y, lineCollider.size.y);

        float angle = Quaternion.FromToRotation(Vector3.up, end.transform.position - front.transform.position).eulerAngles.z;
        lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);

        lineCollider.transform.position = (front.transform.position + end.transform.position) * 0.5f;

    }


    float playerLaserAngle;
    public IEnumerator ControllerPlayerLaser()
    {
        while (true)
        {
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
            }
            yield return null;
        }
    }

    // Temp
    public float SetLaserInfo(LaserType type, float speed = 1.0f, float spriteGap = 0)
    {
        if (this == null) return -1;

        activeLaserFront = true;

        laserFrontSpeed = laserEndSpeed = speed * 2 * PublicValueStorage.Instance.GetAddSpeedRivisionValue();
        laserEndLaunchDelay = speed;


        lineCollider = line.GetComponent<BoxCollider2D>();
        float frontWidth = front.GetComponent<SpriteRenderer>().bounds.extents.y * 2.0f;
        lineCollider.transform.position = front.transform.position;
        lineCollider.size = new Vector2(frontWidth, frontWidth);

        laserType = type;
        switch (type)
        {
            case LaserType.NormalLaser:
                this.tag = "NormalLaser";
                //Debug.Log(this.transform.position + "//" + playerPos);                
                laserFrontVector = laserEndVector = oPCurves.SeekDirection(this.transform.position, playerPos);
                laserEndLaunchDelay *= 0.5f;

                line.SetPosition(0, this.transform.position);
                line.SetPosition(line.positionCount - 1, this.transform.position);
        
                break;

            case LaserType.PlayerLaser:
                this.tag = "PlayerReflectLaser";
                endInitPosition = end.transform.position;
                //Debug.Log(this.transform.position + "//" + playerPos);

                laserFrontSpeed = laserEndSpeed = speed * 3.0f;
                laserEndLaunchDelay /= 3.0f;

                GameObject target = PublicValueStorage.Instance.GetBossChildForAttacked();


                if (target == null)
                {
                    //Debug.Log("is NULL");
                    Vector3 tempDest = new Vector3(0, 10, 0);
                    tempDest.x += Random.Range(-5.0f, 5.0f);
                    laserFrontVector = laserEndVector = oPCurves.SeekDirection(this.transform.position, tempDest);
                }
                else
                {
                    laserFrontVector = laserEndVector = oPCurves.SeekDirection(this.transform.position, target.transform.position);
                }
                //StartCoroutine(ControllerPlayerLaser());

                line.SetPosition(0, this.transform.position);
                line.SetPosition(line.positionCount - 1, this.transform.position);
                break;

            case LaserType.CircleLaser:
                this.tag = "CircleLaser";
                line.enabled = false;
                end.gameObject.SetActive(false);

                circleLaserScale = 1.5f * PublicValueStorage.Instance.GetAddSpeedRivisionValue();
                circleLaserScale = (circleLaserScale >= 2.5f) ? 2.5f : circleLaserScale;

                laserFrontVector = laserEndVector = oPCurves.SeekDirection(this.transform.position, playerPos);

                activeLaserFront = false;
                activeCircleLaser = true;
                break;

            case LaserType.ReflectLaser:
                return -1;

            case LaserType.LaserNo5:
                this.tag = "LaserNo5Laser";
                Vector2 newDestination = playerPos;
                newDestination.x += Random.Range(spriteGap * (-1.2f), spriteGap * 1.2f);
                laserFrontVector = laserEndVector = oPCurves.SeekDirection(this.transform.position, newDestination);

                line.SetPosition(0, this.transform.position);
                line.SetPosition(line.positionCount - 1, this.transform.position);
                
                laserEndLaunchDelay /= speed * 2.5f;
                laserFrontSpeed = laserEndSpeed = speed * 0.5f;
                break;

            default:
                //Debug.Log(this.GetType().ToString() + " is wrong");
                return -1;
        }
        
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).tag = this.tag;
        }
        return laserEndLaunchDelay;
    }

    public void ReflectNormalLaser()
    {

    }
    private void OnDestroy()
    {
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
    }

    public void CreateLaserForPlayer(Vector3 contactPosition)
    {
        //Debug.Log("만든다");
        LaserVer2 newPlayerLaser = Instantiate(laserPrefab, contactPosition, Quaternion.Euler(Vector3.zero));
        newPlayerLaser.SetLaserInfo(LaserType.PlayerLaser);
        //newPlayerLaser.transform.position = playerPos;
        newPlayerLaser.gameObject.SetActive(true);

        PublicValueStorage.Instance.AddMissileScore();
        SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundPlayerLaser);
        //Debug.Log("다만듬 위치 : " + newPlayerLaser.transform.position);
    }
}
