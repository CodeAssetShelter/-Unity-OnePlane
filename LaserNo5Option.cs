using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserNo5Option : MonoBehaviour
{
    enum OptionState { Spawned = 0, OnPosition, Idle, Attack, Launching, MoveBack, Cooldown, StateCount }
    OptionState optionState;

    enum AttackPattern { ReflectLaser = 0, RapidLaser, LaserNo5Laser, MoveCenter, Idle, StateCount }
    AttackPattern attackPattern;

    [Header("- Prefabs")]
    public Laser laserPrefab;
    public Slider hpBar;

    [Header("- Speed values")]
    public float spawnSpeed = 0.8f;
    private float spawnCurrentSpeed = 0;
    public float speedToCenter = 1.5f;
    private float currentSpeedToCenter = 0;
    private float shotSpeedRivision = 1.0f;


    [Header("- Status values")]
    public float hp;



    [Header("- Etc.")]
    public OPCurves opCurves;
    private string myTag = "None";


    // Etc Private values
    private LineRenderer lineRenderer;
    private BoxCollider2D lineCollider;
    private SpriteRenderer startPoint;
    private SpriteRenderer endPoint;
    private SpriteRenderer optionRenderer;

    private float lineWidthStart = 1.0f;
    private float lineWidthEnd = 1.0f;

    private Transform spawnPositionStart;
    private Vector2 myPosition = Vector2.zero;
    private Transform playerPosition;
    private Vector2 direction = Vector2.down;
    private Vector2 screenCenter;


    // Laser Timers
    private float laserTimerMin = 0.2f;
    private float laserTimerMax = 1.0f;
    private float laserTimerCheck = 0;
    private float laserCurrentTimer = 0;
    private float laserCooldownTimerCheck = 0;

    // Laser launch triggers
    public bool activeLaser = false;
    private float angle;
    private MissileModuleLaserNo5 parentScript;

    // Rapid Laser values
    private Quaternion optionRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion myQuaternionRotation = Quaternion.Euler(0, 0, 0);
    private float rotationSpeed = 2.0f;
    private float rotationSpeedToReady = 0;
    private bool rapidLaserNowLaunching = false;

    void Start()
    {
        // Set temporary camera positon in PublicValueSettings
        parentScript = this.transform.GetComponentInParent<MissileModuleLaserNo5>();

        optionRenderer = this.GetComponent<SpriteRenderer>();

        hpBar = Instantiate(hpBar, PublicValueStorage.Instance.GetCanvas().transform);

        float optionHp = PublicValueStorage.Instance.GetBossHp() * 0.1f;
        if (optionHp <= 1.0f)
            optionHp = 10;

        hpBar.maxValue = hpBar.value = optionHp;
        //hpBar.transform.position = this.transform.position;
        hpBar.gameObject.SetActive(false);
        //InitLaserVector();
    }

    //private void SetHpBarColor()
    //{
    //    bossShieldColor.disabledColor = Color.Lerp(Color.red, Color.green, bossShieldSlider.value / bossShieldSlider.maxValue);
    //    bossShieldSlider.colors = bossShieldColor;
    //}

    // Update is called once per frame
    void Update()
    {
        //if (optionState != OptionState.Spawned)
        //{
        //    optionRotation = Quaternion.FromToRotation(Vector3.up, this.transform.position - playerPosition.position).eulerAngles.z;
        //    this.transform.rotation = Quaternion.Euler(0, 0, optionRotation);
        //}


        if (optionState == OptionState.Spawned)
        {
            spawnCurrentSpeed += Time.deltaTime * spawnSpeed * 2.0f;
            this.transform.position = Vector2.Lerp(myPosition, spawnPositionStart.position, spawnCurrentSpeed);

            if (spawnCurrentSpeed >= 1.0f)
            {
                optionState = OptionState.OnPosition;
                spawnCurrentSpeed = 0;

                hpBar.transform.position = this.transform.position;
                hpBar.transform.position = new Vector2(hpBar.transform.position.x, hpBar.transform.position.y + (optionRenderer.bounds.extents.y * 0.5f));
                hpBar.gameObject.SetActive(true);
            }
        }
        if (optionState == OptionState.OnPosition)
        {
            //Debug.Log("On Position!");
            optionState = OptionState.Idle;
        }
        if (optionState == OptionState.Idle)
        {

        }
        if (optionState == OptionState.Launching)
        {
            if (currentSpeedToCenter >= 1.0f)
            {
                currentSpeedToCenter += speedToCenter * Time.deltaTime;

                switch (attackPattern)
                {
                    // Options on Postion
                    case AttackPattern.ReflectLaser:
                        this.transform.rotation = Quaternion.Lerp(myQuaternionRotation, optionRotation, currentSpeedToCenter);
                        break;

                    // Options on Postion
                    case AttackPattern.RapidLaser:
                        this.transform.rotation = Quaternion.Lerp(myQuaternionRotation, optionRotation, currentSpeedToCenter);
                        this.transform.position = Vector2.Lerp(spawnPositionStart.position, screenCenter, currentSpeedToCenter);
                        //this.transform.rotation = Quaternion.Euler(0, 0, optionRotation);
                        break;

                    
                    case AttackPattern.LaserNo5Laser:
                        break;


                    default:
                        Debug.LogError("Attack error");
                        break;
                }
                if (currentSpeedToCenter >= 1.0f)
                {
                    optionState = OptionState.Attack;
                    currentSpeedToCenter = 0;
                }
            }
            //else
            //{
            //    currentSpeedToCenter = speedToCenter * Time.deltaTime;
            //    this.transform.position = Vector2.Lerp(spawnPositionStart.position, screenCenter, currentSpeedToCenter);
            //}
        }
        if (optionState == OptionState.Attack)
        {
            optionState = OptionState.Idle;
            GameObject laser;
            Laser script;
            switch (attackPattern)
            {
                // Options on Postion
                case AttackPattern.ReflectLaser:
                    laser = Instantiate(laserPrefab.gameObject, this.transform.position, Quaternion.Euler(0, 0, 0));
                    script = laser.GetComponent<Laser>();
                    laserCurrentTimer = Random.Range(laserTimerMin, laserTimerMax);
                    script.InitLaserVector(myTag, shotSpeedRivision);
                    script.SetStartEndPointTimer(laserCurrentTimer);
                    optionState = OptionState.Cooldown;
                    break;

                // Options on Postion
                case AttackPattern.RapidLaser:
                    laser = Instantiate(laserPrefab.gameObject, this.transform.position, Quaternion.Euler(0, 0, 0));
                    script = laser.GetComponent<Laser>();
                    laserCurrentTimer = Random.Range(laserTimerMin, laserTimerMax);
                    script.InitLaserVector(myTag, shotSpeedRivision);
                    script.SetStartEndPointTimer(laserCurrentTimer);
                    optionState = OptionState.MoveBack;
                    break;


                case AttackPattern.LaserNo5Laser:
                    laser = Instantiate(laserPrefab.gameObject, this.transform.position, Quaternion.Euler(0, 0, 0));
                    script = laser.GetComponent<Laser>();
                    script.InitLaserVector(myTag, shotSpeedRivision);
                    script.SetStartEndPointTimer(laserCurrentTimer);
                    break;

                default:
                    Debug.LogError("Attack error");
                    break;
            }
        }
        if (optionState == OptionState.MoveBack)
        {
            currentSpeedToCenter += speedToCenter * Time.deltaTime;
            this.transform.position = Vector2.Lerp(screenCenter, spawnPositionStart.position, currentSpeedToCenter);
            this.transform.rotation = Quaternion.Lerp(optionRotation, myQuaternionRotation, currentSpeedToCenter);

            if (currentSpeedToCenter >= 1.0f)
            {
                rotationSpeedToReady = 0;
                optionState = OptionState.Idle;
            }
        }
        if (optionState == OptionState.Cooldown)
        {
            laserCooldownTimerCheck += Time.deltaTime;
            if (laserCooldownTimerCheck >= laserCurrentTimer)
            {
                laserCooldownTimerCheck = 0;
                optionState = OptionState.Idle;
            }
        }
        //if (activeLaser == true)
        //{
        //    endPoint.transform.Translate(direction * Time.deltaTime, Space.World);
        //    ActivateLaser();
        //}
    }


    //private void InitLaserVector()
    //{
    //    //eu = Quaternion.FromToRotation(Vector3.up, startPoint.transform.position - endPoint.transform.position).eulerAngles.z;
    //    lineWidthStart = startPoint.bounds.extents.x * 2.0f;
    //    lineWidthEnd = endPoint.bounds.extents.x * 2.0f;
    //    lineRenderer.startWidth = lineWidthStart;
    //    lineRenderer.endWidth = lineWidthEnd;

    //    lineCollider.transform.position = startPoint.transform.position;
    //    lineCollider.size = new Vector2(lineWidthStart, startPoint.bounds.extents.y * 2.0f);

    //    direction = opCurves.SeekDirection(endPoint.transform.position, playerPosition.position);

    //    angle = Quaternion.FromToRotation(Vector3.up, startPoint.transform.position - (Vector3)playerPosition.position).eulerAngles.z;

    //    lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);
    //    startPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
    //    endPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
    //}

    //private void ActivateLaser()
    //{
    //    //eu = Quaternion.FromToRotation(Vector3.up, startPoint.transform.position - endPoint.transform.position).eulerAngles.z;

    //    //LineCollider.transform.rotation = Quaternion.Euler(0, 0, eu - 90.0f);
    //    float size = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
    //    lineCollider.size = new Vector2(size + lineCollider.size.y, lineCollider.size.y);

    //    lineCollider.transform.position = (startPoint.transform.position + endPoint.transform.position) * 0.5f;
    //    //startPoint.transform.rotation = Quaternion.Euler(0, 0, eu);
    //    //endPoint.transform.rotation = Quaternion.Euler(0, 0, eu);

    //    lineRenderer.SetPosition(0, startPoint.transform.position);
    //    lineRenderer.SetPosition(1, endPoint.transform.position);
    //}


    public void InitOption(Vector2 myPosition, Transform spawnStartPos, Transform player)
    {
        this.myPosition = this.transform.position = myPosition;
        this.gameObject.SetActive(true);
        spawnPositionStart = spawnStartPos;
        playerPosition = player;
        optionState = OptionState.Spawned;

        screenCenter = playerPosition.position;
        screenCenter.y = Screen.height * 0.7f;
        //Debug.Log("my pos : " + this.transform.position);
    }


    //public void activateLaser(bool active)
    //{
    //    activeLaser = active;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern">
    /// 0. ReflectLaser<para /> 
    /// 1. RapidLaser<para />
    /// 2. LaserNo5Laser<para />
    /// </param>
    public void SetLaserPattern(int pattern, float laserNo5Timer = 0)
    {
        if (optionState == OptionState.Idle)
        {
            float z = Quaternion.FromToRotation(Vector3.up, this.transform.position - playerPosition.position).eulerAngles.z;
            optionRotation = Quaternion.Euler(0, 0, z);
            myQuaternionRotation = this.transform.rotation;
            switch (pattern)
            {
                case 0:
                    myTag = "ReflectLaser";
                    startPoint.gameObject.tag = endPoint.gameObject.tag = myTag;
                    attackPattern = AttackPattern.ReflectLaser;
                    break;
                case 1:
                    myTag = "RapidLaser";
                    startPoint.gameObject.tag = endPoint.gameObject.tag = myTag;
                    //this.transform.rotation = Quaternion.Euler(0, 0, optionRotation);
                    attackPattern = AttackPattern.RapidLaser;
                    break;
                case 2:
                case 3:
                    myTag = "LaserNo5Laser";
                    startPoint.gameObject.tag = endPoint.gameObject.tag = myTag;
                    laserCurrentTimer = laserNo5Timer;
                    attackPattern = AttackPattern.LaserNo5Laser;
                    break;


            }
            optionState = OptionState.Launching;
        }
        else
        {
            Debug.Log("State is not Idle");
            return;
        }
    }

    public void SetSpeedRivision(float speed)
    {
        shotSpeedRivision = speed;
    }

    public void GetDamaged()
    {
        hpBar.value = --hp;
        //SetBossHpBarColor();
        if (hp <= 0)
        {
            // Do Something If boss died
            Debug.Log("Option is dead");
            OptionDead();
        }
    }

    private void OptionDead()
    {
        this.gameObject.SetActive(false);
        this.transform.position = parentScript.transform.position;
        //parentScript.CheckAllOptionsDestroyed();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerLaser")
        {
            // do something
            GetDamaged();
        }
    }
}
